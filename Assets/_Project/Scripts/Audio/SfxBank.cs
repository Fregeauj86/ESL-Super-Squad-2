using System.Collections.Generic;
using UnityEngine;

namespace FromCell.Audio
{
    /// <summary>
    /// Named, lazily-baked procedural sound effects - one AudioClip built (and cached) per
    /// key on first request. Note/duration/gain constants are ported directly from each
    /// matching method in web/js/audio.js's GameAudio class (playJump, playCollect, etc.);
    /// ChallengePass/ChallengeFail have no web-version counterpart (that game has no ESL
    /// challenge system) and are new stingers built from the same Tone/Chord primitives.
    /// </summary>
    public static class SfxBank
    {
        static readonly Dictionary<string, AudioClip> cache = new Dictionary<string, AudioClip>();

        public const string Jump = "jump";
        public const string DoubleJump = "double_jump";
        public const string Dash = "dash";
        public const string Collect = "collect";
        public const string Growth = "growth";
        public const string Checkpoint = "checkpoint";
        public const string Respawn = "respawn";
        public const string Finish = "finish";
        public const string Evolution = "evolution";
        public const string ChallengePass = "challenge_pass";
        public const string ChallengeFail = "challenge_fail";

        public static AudioClip Get(string key)
        {
            if (cache.TryGetValue(key, out var cached) && cached != null)
                return cached;

            var clip = Build(key);
            cache[key] = clip;
            return clip;
        }

        static AudioClip Build(string key)
        {
            switch (key)
            {
                case Jump:
                    return ProceduralSfx.Tone(540f, 0.08f, WaveType.Square, 0.1f, slide: 140f);
                case DoubleJump:
                    return ProceduralSfx.Chord(new[] { 620f, 780f }, 0.09f, WaveType.Triangle, 0.09f);
                case Dash:
                    return ProceduralSfx.Tone(180f, 0.11f, WaveType.Sawtooth, 0.12f, slide: -40f);
                case Collect:
                    return ProceduralSfx.Chord(new[] { 740f, 990f }, 0.08f, WaveType.Triangle, 0.1f);
                case Growth:
                    return ProceduralSfx.Chord(new[] { 330f, 440f, 554.37f }, 0.12f, WaveType.Sine, 0.11f);
                case Checkpoint:
                    return ProceduralSfx.Tone(880f, 0.07f, WaveType.Triangle, 0.08f);
                case Respawn:
                    return ProceduralSfx.Tone(220f, 0.18f, WaveType.Sine, 0.11f, slide: -90f);
                case Finish:
                    return ProceduralSfx.Chord(new[] { 523.25f, 659.25f, 783.99f }, 0.18f, WaveType.Triangle, 0.14f);
                case Evolution:
                    return ProceduralSfx.Chord(new[] { 262f, 392f, 523.25f }, 0.18f, WaveType.Sine, 0.12f);
                case ChallengePass:
                    return ProceduralSfx.Chord(new[] { 523.25f, 659.25f, 987.77f }, 0.16f, WaveType.Triangle, 0.13f);
                case ChallengeFail:
                    return ProceduralSfx.Tone(220f, 0.22f, WaveType.Sawtooth, 0.12f, slide: -120f);
                default:
                    return null;
            }
        }
    }
}
