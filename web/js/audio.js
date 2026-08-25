const clamp = (value, min, max) => Math.min(max, Math.max(min, value));

export class GameAudio {
  constructor() {
    this.context = null;
    this.master = null;
    this.unlocked = false;
  }

  unlock() {
    if (this.unlocked) return;
    const AudioContextCtor = window.AudioContext || window.webkitAudioContext;
    if (!AudioContextCtor) return;

    this.context = this.context || new AudioContextCtor();
    this.master = this.master || this.context.createGain();
    this.master.gain.value = 0.12;
    this.master.connect(this.context.destination);
    this.context.resume();
    this.unlocked = true;
  }

  _tone({ freq = 440, duration = 0.12, type = 'sine', gain = 0.18, slide = 0, detune = 0 }) {
    if (!this.unlocked || !this.context) return;

    const now = this.context.currentTime;
    const osc = this.context.createOscillator();
    const amp = this.context.createGain();
    osc.type = type;
    osc.frequency.setValueAtTime(freq, now);
    if (slide !== 0) {
      osc.frequency.exponentialRampToValueAtTime(clamp(freq + slide, 40, 2400), now + duration);
    }
    osc.detune.setValueAtTime(detune, now);
    amp.gain.setValueAtTime(0.0001, now);
    amp.gain.exponentialRampToValueAtTime(gain, now + 0.015);
    amp.gain.exponentialRampToValueAtTime(0.0001, now + duration);
    osc.connect(amp);
    amp.connect(this.master);
    osc.start(now);
    osc.stop(now + duration + 0.05);
  }

  _chord(notes, options) {
    notes.forEach((freq, index) => {
      this._tone({ ...options, freq, detune: index * 6 - 6 });
    });
  }

  playStart() {
    this._chord([392, 523.25, 659.25], { duration: 0.16, gain: 0.14, type: 'triangle' });
  }

  playLevelStart(stageName) {
    const base = stageName === 'Adult' ? 440 : 330;
    this._chord([base, base * 1.25, base * 1.5], { duration: 0.14, gain: 0.12, type: 'triangle' });
  }

  playJump() {
    this._tone({ freq: 540, slide: 140, duration: 0.08, gain: 0.1, type: 'square' });
  }

  playDoubleJump() {
    this._chord([620, 780], { duration: 0.09, gain: 0.09, type: 'triangle' });
  }

  playDash() {
    this._tone({ freq: 180, slide: -40, duration: 0.11, gain: 0.12, type: 'sawtooth' });
  }

  playCollect() {
    this._chord([740, 990], { duration: 0.08, gain: 0.1, type: 'triangle' });
  }

  playGrowth() {
    this._chord([330, 440, 554.37], { duration: 0.12, gain: 0.11, type: 'sine' });
  }

  playCheckpoint() {
    this._tone({ freq: 880, duration: 0.07, gain: 0.08, type: 'triangle' });
  }

  playRespawn() {
    this._tone({ freq: 220, slide: -90, duration: 0.18, gain: 0.11, type: 'sine' });
  }

  playFinish() {
    this._chord([523.25, 659.25, 783.99], { duration: 0.18, gain: 0.14, type: 'triangle' });
  }

  playEvolution() {
    this._chord([262, 392, 523.25], { duration: 0.18, gain: 0.12, type: 'sine' });
  }
}