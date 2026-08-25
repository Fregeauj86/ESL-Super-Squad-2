using System;
using UnityEngine;

namespace FromCell.Audio
{
    public enum WaveType { Sine, Square, Triangle, Sawtooth }

    /// <summary>
    /// Bakes short procedural sound effects into real AudioClips via AudioClip.Create +
    /// SetData - ports web/js/audio.js's GameAudio._tone/_chord (a live Web Audio oscillator
    /// + exponential-envelope gain node) sample-by-sample instead, since Unity has no
    /// equivalent live-graph API. Frequency slide is applied via per-sample phase
    /// accumulation (phase += instantaneousFreq/sampleRate each sample) rather than a closed-
    /// form integral of the exponential ramp - numerically equivalent for audio purposes and
    /// far less error-prone to get right by hand. A "chord" mixes several detuned tones into
    /// one clip, matching _chord firing several _tone oscillators together; per-sample
    /// clamping keeps the mix from clipping instead of a separate normalize pass.
    /// </summary>
    public static class ProceduralSfx
    {
        const int SampleRate = 44100;
        const float EnvelopeFloor = 0.0001f;
        const float AttackTime = 0.015f;

        public static AudioClip Tone(float freq, float duration, WaveType type, float gain, float slide = 0f)
        {
            return BuildClip(new[] { (freq, 0f) }, duration, type, gain, slide);
        }

        public static AudioClip Chord(float[] freqs, float duration, WaveType type, float gain, float slide = 0f)
        {
            var notes = new (float freq, float detuneCents)[freqs.Length];
            for (int i = 0; i < freqs.Length; i++)
                notes[i] = (freqs[i], i * 6f - 6f);
            return BuildClip(notes, duration, type, gain, slide);
        }

        static AudioClip BuildClip((float freq, float detuneCents)[] notes, float duration, WaveType type, float gain, float slide)
        {
            int sampleCount = Mathf.Max(1, Mathf.CeilToInt((duration + 0.05f) * SampleRate));
            var samples = new float[sampleCount];

            foreach (var note in notes)
                AddTone(samples, note.freq, note.detuneCents, slide, duration, type, gain);

            for (int i = 0; i < samples.Length; i++)
                samples[i] = Mathf.Clamp(samples[i], -1f, 1f);

            var clip = AudioClip.Create("ProceduralSfx", sampleCount, 1, SampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        static void AddTone(float[] samples, float freq, float detuneCents, float slide, float duration, WaveType type, float gain)
        {
            float f0 = freq * Mathf.Pow(2f, detuneCents / 1200f);
            float f1 = slide != 0f ? Mathf.Clamp(f0 + slide, 40f, 2400f) : f0;
            float attackEnd = Mathf.Min(AttackTime, duration);
            int durationSamples = Mathf.Min(samples.Length, Mathf.CeilToInt(duration * SampleRate));

            double phase = 0.0;
            for (int i = 0; i < durationSamples; i++)
            {
                float t = i / (float)SampleRate;

                float freqAtT = Mathf.Approximately(f1, f0) ? f0 : f0 * Mathf.Pow(f1 / f0, t / duration);
                phase += freqAtT / SampleRate;

                float env = Envelope(t, attackEnd, duration, gain);
                samples[i] += Waveform(type, phase) * env;
            }
        }

        static float Envelope(float t, float attackEnd, float duration, float gain)
        {
            if (t <= attackEnd)
            {
                float u = attackEnd > 0f ? t / attackEnd : 1f;
                return EnvelopeFloor * Mathf.Pow(gain / EnvelopeFloor, u);
            }

            float decayU = Mathf.Clamp01((t - attackEnd) / Mathf.Max(0.0001f, duration - attackEnd));
            return gain * Mathf.Pow(EnvelopeFloor / gain, decayU);
        }

        static float Waveform(WaveType type, double phase)
        {
            double p = phase - Math.Floor(phase);

            switch (type)
            {
                case WaveType.Sine:
                    return (float)Math.Sin(p * Math.PI * 2.0);
                case WaveType.Square:
                    return p < 0.5 ? 1f : -1f;
                case WaveType.Triangle:
                    if (p < 0.25) return (float)(4.0 * p);
                    if (p < 0.75) return (float)(2.0 - 4.0 * p);
                    return (float)(4.0 * p - 4.0);
                case WaveType.Sawtooth:
                    return (float)(2.0 * p - 1.0);
                default:
                    return 0f;
            }
        }
    }
}
