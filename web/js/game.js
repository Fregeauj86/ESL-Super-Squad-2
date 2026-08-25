import { LEVELS } from './levels.js?v=2';
import { getStage } from './evolution.js';
import { Player } from './player.js';
import { Input } from './input.js';
import { saveProgress, loadProgress, clearProgress } from './save.js';
import { Effects } from './effects.js';
import { GameAudio } from './audio.js';

export class Game {
  constructor(canvas) {
    this.canvas = canvas;
    this.ctx = canvas.getContext('2d');
    this.input = new Input();
    this.player = new Player();
    this.levelIndex = 0;
    this.state = 'menu';
    this.tutorialTimer = 0;
    this.blockedMsgTimer = 0;
    this.lastTime = 0;
    this.camX = 0;
    this._lastGrowthCount = 0;
    this._lastCollectibleCount = 0;
    this._lastPowerupCount = 0;
    this._lastRole = null;
    this.elapsed = 0;
    this.effects = new Effects();
    this.audio = new GameAudio();

    this._bindUI();
    window.addEventListener('pointerdown', () => this.audio.unlock(), { once: true });
    window.addEventListener('keydown', () => this.audio.unlock(), { once: true });
    this._resize();
    this._refreshContinueButton();
    window.addEventListener('resize', () => this._resize());
  }

  _bindUI() {
    document.getElementById('btn-start')?.addEventListener('click', () => this.startNewGame());
    document.getElementById('btn-continue')?.addEventListener('click', () => this.continueGame());
    document.getElementById('btn-resume')?.addEventListener('click', () => this.resume());
    document.getElementById('btn-restart')?.addEventListener('click', () => this.restartLevel());
    document.getElementById('btn-menu')?.addEventListener('click', () => this.goMenu());
    document.getElementById('btn-credits-menu')?.addEventListener('click', () => this.goMenu());
  }

  _refreshContinueButton() {
    const btn = document.getElementById('btn-continue');
    if (!btn) return;
    const save = loadProgress();
    btn.classList.toggle('hidden', !save);
    btn.textContent = save
      ? `CONTINUE — ${LEVELS[Math.min(save.levelIndex + 1, LEVELS.length - 1)].name}`
      : 'CONTINUE';
  }

  _resize() {
    const scale = Math.min(window.innerWidth / this.canvas.width, window.innerHeight / this.canvas.height);
    this.canvas.style.width = `${this.canvas.width * scale}px`;
    this.canvas.style.height = `${this.canvas.height * scale}px`;
  }

  get level() { return LEVELS[this.levelIndex]; }

  startNewGame() {
    clearProgress();
    this.levelIndex = 0;
    this.audio.unlock();
    this._startPlaying();
  }

  continueGame() {
    const save = loadProgress();
    if (!save) return;
    this.levelIndex = Math.min(save.levelIndex + 1, LEVELS.length - 1);
    this.audio.unlock();
    this._startPlaying();
  }

  _startPlaying() {
    this._loadLevel();
    this.state = 'playing';
    this.audio.playStart();
    document.getElementById('menu-overlay')?.classList.add('hidden');
    document.getElementById('credits-overlay')?.classList.add('hidden');
    document.getElementById('touch-controls')?.classList.remove('hidden');
  }

  goMenu() {
    this.state = 'menu';
    document.getElementById('menu-overlay')?.classList.remove('hidden');
    document.getElementById('pause-overlay')?.classList.add('hidden');
    document.getElementById('credits-overlay')?.classList.add('hidden');
    document.getElementById('touch-controls')?.classList.add('hidden');
    this._refreshContinueButton();
  }

  pause() {
    if (this.state !== 'playing') return;
    this.state = 'paused';
    const label = document.getElementById('pause-level-label');
    if (label) label.textContent = this.level.name;
    document.getElementById('pause-overlay')?.classList.remove('hidden');
  }

  resume() {
    if (this.state !== 'paused') return;
    this.state = 'playing';
    document.getElementById('pause-overlay')?.classList.add('hidden');
  }

  restartLevel() {
    document.getElementById('pause-overlay')?.classList.add('hidden');
    this._loadLevel();
    this.state = 'playing';
  }

  _loadLevel() {
    const level = this.level;
    level.collectibles.forEach((c) => { c.taken = false; });
    (level.growth || []).forEach((g) => { g.taken = false; });
    (level.powerups || []).forEach((p) => { p.taken = false; });
    (level.enemies || []).forEach((e) => {
      e.dir = e.dir || 1;
      e._spawnX = e._spawnX ?? e.x;
      e._spawnY = e._spawnY ?? e.y;
      e.x = e._spawnX;
      e.y = e._spawnY;
    });
    const stage = getStage(level.stageIndex);
    this.player.applyStage(stage);
    this.player.reset(level.spawn.x, level.spawn.y);
    this.player.setCheckpoint(level.spawn.x, level.spawn.y);
    this.tutorialTimer = 5;
    this.camX = Math.max(0, level.spawn.x - 320);
    this._updateHud(stage, level);
    this._showTutorial(level.tutorial);
    this._toggleDashButton(stage.canDash);
    this._updateJumpButton(stage);
    this._updateDashCooldown(1);
    this.player.snapToGround(level);
    this._lastGrowthCount = 0;
    this._lastCollectibleCount = 0;
    this._lastPowerupCount = 0;
    this._lastRole = null;
    this.effects.items = [];
    for (const p of level.platforms) {
      if (p.gateRole) p.gateOpen = false;
    }
    this.audio.playLevelStart(stage.name);
  }

  _showTutorial(text) {
    const el = document.getElementById('tutorial');
    if (!el) return;
    el.textContent = text;
    el.classList.remove('hidden');
  }

  _toggleDashButton(show) {
    document.getElementById('btn-dash')?.classList.toggle('hidden', !show);
    document.getElementById('dash-cooldown')?.classList.toggle('hidden', !show);
  }

  _updateJumpButton(stage) {
    const btn = document.getElementById('btn-jump');
    if (!btn) return;
    const active = stage.canJump && stage.jumpForce > 0;
    btn.classList.toggle('disabled', !active);
    btn.title = active ? 'Jump' : 'Jump unlocks in later evolution stages';
  }

  _updateDashCooldown(norm) {
    const bar = document.getElementById('dash-cooldown-fill');
    if (bar) bar.style.transform = `scaleX(${norm})`;
  }

  _updateEnemies(level, dt) {
    for (const enemy of level.enemies || []) {
      const minX = typeof enemy.minX === 'number' ? enemy.minX : enemy._spawnX - 60;
      const maxX = typeof enemy.maxX === 'number' ? enemy.maxX : enemy._spawnX + 60;
      enemy.x += (enemy.speed || 0) * (enemy.dir || 1) * dt;
      if (enemy.x <= minX) { enemy.x = minX; enemy.dir = 1; }
      if (enemy.x >= maxX) { enemy.x = maxX; enemy.dir = -1; }
      if (enemy.float) {
        enemy.y = enemy._spawnY + Math.sin(this.elapsed * (enemy.floatSpeed || 2) + (enemy.phase || 0)) * (enemy.floatRange || 6);
      }
      if (this.player.invulnTime <= 0 && this.player.stage && this.player._hitsBox(enemy, this.player._currentRadius())) {
        this.player.respawn(level);
        this._showTutorial('Hit by an enemy. Back at the last checkpoint.');
        this.blockedMsgTimer = 1.5;
        break;
      }
    }
  }

  _updateHud(stage, level) {
    document.getElementById('stage-label').textContent = stage.name;
    document.getElementById('level-label').textContent = level.name;
  }

  _collectibleCount() {
    return this.level.collectibles.filter((c) => c.taken).length;
  }

  _growthCount() {
    return (this.level.growth || []).filter((g) => g.taken).length;
  }

  _powerupCount() {
    return (this.level.powerups || []).filter((p) => p.taken).length;
  }

  _canCompleteLevel() {
    const level = this.level;
    return this._collectibleCount() >= (level.requiredCollectibles || 0) &&
      this._growthCount() >= (level.requiredGrowth || 0);
  }

  completeLevel() {
    if (!this._canCompleteLevel()) {
      const level = this.level;
      const need = level.requiredCollectibles || 0;
      const needGrowth = level.requiredGrowth || 0;
      if (this._collectibleCount() < need) {
        this._showTutorial(`Collect ${need} nutrients before exiting.`);
      } else if (this._growthCount() < needGrowth) {
        this._showTutorial(`Collect all ${needGrowth} growth orbs to mature enough to leave.`);
      }
      this.blockedMsgTimer = 2;
      return;
    }

    saveProgress(this.levelIndex);
    this.audio.playEvolution();
    this.audio.playFinish();

    const nextStage = getStage(Math.min(this.level.stageIndex + 1, 9));
    document.getElementById('evo-title').textContent = nextStage.name;
    document.getElementById('evo-humor').textContent = nextStage.humor;
    document.getElementById('evolution-overlay')?.classList.remove('hidden');
    this.state = 'evolving';

    if (this.levelIndex >= LEVELS.length - 1) {
      setTimeout(() => {
        document.getElementById('evolution-overlay')?.classList.add('hidden');
        document.getElementById('credits-overlay')?.classList.remove('hidden');
        this.state = 'credits';
      }, 2200);
      return;
    }

    setTimeout(() => {
      this.levelIndex++;
      document.getElementById('evolution-overlay')?.classList.add('hidden');
      this._loadLevel();
      this.state = 'playing';
    }, 2200);
  }

  update(dt) {
    if (this.state === 'menu' || this.state === 'credits') return;

    if (this.input.consumePause()) {
      if (this.state === 'playing') this.pause();
      else if (this.state === 'paused') this.resume();
    }

    if (this.state === 'paused' || this.state === 'evolving') return;

    const level = this.level;
    this.elapsed += dt;
    this.player.update(dt, this.input, level);
    this._updateEnemies(level, dt);

    for (const p of level.platforms) {
      if (p.gateRole) p.gateOpen = this.player.role === p.gateRole;
    }

    const growthNow = this._growthCount();
    if (growthNow > this._lastGrowthCount) {
      this._showTutorial('Growth orb absorbed. Movement feels lighter.');
      this.blockedMsgTimer = 2;
      this._updateJumpButton(this.player.stage);
      this.effects.burst(this.player.x, this.player.y, '#f078a8', 10);
      this.audio.playGrowth();
    }
    this._lastGrowthCount = growthNow;

    const colNow = this._collectibleCount();
    if (colNow > this._lastCollectibleCount) {
      this.effects.burst(this.player.x, this.player.y, '#f0d848', 8);
      const need = level.requiredCollectibles || 0;
      if (need) {
        this._showTutorial(`Nutrient secured: ${colNow} / ${need}.`);
        this.blockedMsgTimer = 1.5;
      }
      this.audio.playCollect();
    }
    this._lastCollectibleCount = colNow;

    const powerNow = this._powerupCount();
    if (powerNow > this._lastPowerupCount) {
      const kind = this.player.powerupKind || 'boost';
      this._showTutorial(kind === 'shield' ? 'Shield power-up absorbed.' : kind === 'jump' ? 'Jump power-up absorbed.' : 'Speed power-up absorbed.');
      this.blockedMsgTimer = 1.5;
      this.effects.burst(this.player.x, this.player.y, kind === 'shield' ? '#8fd4ff' : kind === 'jump' ? '#f0a8ff' : '#8ff0b8', 10);
      this.audio.playCollect();
    }
    this._lastPowerupCount = powerNow;

    if (this.player.justCheckpoint) {
      this.player.justCheckpoint = false;
      this._showTutorial('Checkpoint locked in.');
      this.blockedMsgTimer = 1.5;
      this.effects.burst(this.player.x, this.player.y, '#64c8ff', 6);
      this.audio.playCheckpoint();
    }

    if (this.player.justRespawned) {
      this.player.justRespawned = false;
      this._showTutorial('Back at the last checkpoint.');
      this.blockedMsgTimer = 1.5;
      this.audio.playRespawn();
    }

    if (this.player.justJumped) {
      this.audio.playJump();
    }

    if (this.player.justDoubleJumped) {
      this.audio.playDoubleJump();
    }

    if (this.player.justDashed) {
      this.audio.playDash();
    }

    if (this.player.justPowerup) {
      this.player.justPowerup = false;
      this.player.powerupKind = null;
    }

    if (level.rolePads?.length && this.player.role !== this._lastRole) {
      if (this.player.role) {
        this._showTutorial(`${this.player.role} role active — pass matching gates.`);
        this.blockedMsgTimer = 2;
      }
      this._lastRole = this.player.role;
    }

    if (this.player.jumpDenied) {
      this.blockedMsgTimer = 1.2;
      this._showTutorial(this.player.stage?.canJump === false
        ? 'Jump is locked for this stage. Keep drifting forward.'
        : 'Need solid ground to jump.');
    }

    if (this.tutorialTimer > 0) {
      this.tutorialTimer -= dt;
      if (this.tutorialTimer <= 0 && this.blockedMsgTimer <= 0) {
        document.getElementById('tutorial')?.classList.add('hidden');
      }
    }
    if (this.blockedMsgTimer > 0) {
      this.blockedMsgTimer -= dt;
      if (this.blockedMsgTimer <= 0) document.getElementById('tutorial')?.classList.add('hidden');
    }

    const col = this._collectibleCount();
    const need = level.requiredCollectibles;
    const growth = this._growthCount();
    const needGrowth = level.requiredGrowth || 0;
    const parts = [];
    if (need) parts.push(`Nutrients ${col} / ${need}`);
    else if (col) parts.push(`Nutrients ${col}`);
    if (needGrowth) parts.push(`Growth Orbs ${growth} / ${needGrowth}`);
    else if ((level.growth || []).length) parts.push(`Growth Orbs ${growth}`);
    if ((level.powerups || []).length) parts.push(`Power-ups ${this._powerupCount()} / ${level.powerups.length}`);
    if (level.rolePads?.length) {
      parts.push(this.player.role ? `Role: ${this.player.role}` : 'Role: —');
    }
    document.getElementById('collect-label').textContent = parts.join(' · ');

    if (level.stageIndex >= 8) {
      this._updateDashCooldown(this.player.dashReady);
      document.getElementById('btn-dash')?.classList.toggle('cooling', this.player.dashReady < 1);
    }

    this.effects.update(dt);

    const worldW = level.worldWidth || 960;
    const targetCam = Math.max(0, Math.min(this.player.x - 320, worldW - 640));
    this.camX += (targetCam - this.camX) * Math.min(1, dt * 6);

    if (this.player.hitsFinish(level)) this.completeLevel();
  }

  _drawBackdrop(ctx, level, w, h, cam) {
    const stageIndex = level.stageIndex || 0;
    const palettes = [
      ['#07151d', '#0e2a37', '#163542'],
      ['#08191a', '#12303a', '#1e443f'],
      ['#12151f', '#1c2940', '#26384f'],
      ['#181513', '#352a1e', '#4a3927'],
      ['#1a1019', '#35213b', '#4b3150'],
      ['#10161f', '#1a2440', '#27324f'],
      ['#150f0d', '#2d201a', '#46342a'],
      ['#111821', '#24354d', '#314763'],
      ['#0f1420', '#202b44', '#2f4061'],
      ['#0f1218', '#1d2430', '#2b3440'],
      ['#090f16', '#161f2d', '#223041'],
    ];
    const palette = palettes[stageIndex] || palettes[0];
    const grad = ctx.createLinearGradient(0, 0, 0, h);
    grad.addColorStop(0, palette[0]);
    grad.addColorStop(0.65, palette[1]);
    grad.addColorStop(1, palette[2]);
    ctx.fillStyle = grad;
    ctx.fillRect(0, 0, w, h);

    ctx.fillStyle = 'rgba(255,255,255,0.04)';
    for (let i = 0; i < 34; i++) {
      const px = (i * 97 + cam * 0.08) % (w + 160) - 80;
      const py = 28 + ((i * 53 + stageIndex * 29) % 220);
      const size = 1 + (i % 3);
      ctx.fillRect(px, py, size, size);
    }

    const horizon = h * 0.7;
    ctx.fillStyle = stageIndex < 3 ? 'rgba(120, 220, 180, 0.12)' : stageIndex < 6 ? 'rgba(180, 140, 220, 0.12)' : 'rgba(220, 190, 160, 0.12)';
    for (let band = 0; band < 3; band++) {
      const offset = band * 52;
      ctx.beginPath();
      ctx.moveTo(0, horizon + offset);
      for (let x = -60; x <= w + 60; x += 120) {
        const wave = Math.sin((cam * 0.003) + x * 0.01 + band) * (12 + band * 4);
        ctx.lineTo(x, horizon + offset + wave);
      }
      ctx.lineTo(w, h);
      ctx.lineTo(0, h);
      ctx.closePath();
      ctx.fill();
    }

    const orbColor = stageIndex < 3 ? 'rgba(110, 240, 200, 0.18)' : stageIndex < 6 ? 'rgba(240, 160, 200, 0.16)' : 'rgba(160, 200, 255, 0.16)';
    const orbs = [
      { x: 0.14, y: 0.2, r: 120 },
      { x: 0.72, y: 0.16, r: 160 },
      { x: 0.84, y: 0.42, r: 90 },
    ];
    for (const orb of orbs) {
      const cx = orb.x * w + Math.sin((cam * 0.001) + orb.r) * 12;
      const cy = orb.y * h;
      const glow = ctx.createRadialGradient(cx, cy, 0, cx, cy, orb.r);
      glow.addColorStop(0, orbColor);
      glow.addColorStop(1, 'rgba(0,0,0,0)');
      ctx.fillStyle = glow;
      ctx.beginPath();
      ctx.arc(cx, cy, orb.r, 0, Math.PI * 2);
      ctx.fill();
    }
  }

  draw() {
    const ctx = this.ctx, w = this.canvas.width, h = this.canvas.height;
    const level = this.level;
    this._drawBackdrop(ctx, level, w, h, this.camX);

    if (this.state === 'menu' || this.state === 'credits') return;

    const cam = this.camX;

    for (const wn of level.winds || (level.wind ? [level.wind] : [])) {
      ctx.fillStyle = 'rgba(80, 160, 220, 0.14)';
      ctx.fillRect(wn.x - cam, wn.y, wn.w, wn.h);
      ctx.fillStyle = 'rgba(120, 200, 255, 0.2)';
      for (let i = 0; i < 5; i++) {
        const ox = ((Date.now() / 40 + i * 80) % wn.w);
        const oy = wn.fy && wn.fy < 0 ? 20 + i * 36 : 40 + i * 40;
        ctx.fillRect(wn.x - cam + ox, wn.y + oy, 30, 4);
      }
      if (wn.fy && wn.fy < 0) {
        ctx.fillStyle = 'rgba(120, 200, 255, 0.15)';
        for (let i = 0; i < 4; i++) {
          const ox = ((Date.now() / 55 + i * 100) % wn.w);
          ctx.fillRect(wn.x - cam + ox, wn.y + wn.h - 30 - i * 28, 4, 22);
        }
      }
    }

    for (const pad of level.rolePads || []) {
      ctx.fillStyle = pad.role === 'nerve' ? 'rgba(80,120,220,0.7)' : 'rgba(220,100,80,0.7)';
      ctx.fillRect(pad.x - cam, pad.y, pad.w, pad.h);
    }

    for (const p of level.platforms) {
      if (p.gateRole && !p.gateOpen) {
        ctx.fillStyle = '#6a3a8a';
        ctx.globalAlpha = 0.85;
      } else if (p.gateRole && p.gateOpen) {
        ctx.fillStyle = '#5a9a7a';
        ctx.globalAlpha = 0.45;
      } else {
        ctx.fillStyle = p.color || '#4a7a65';
        ctx.globalAlpha = 1;
      }
      ctx.fillRect(p.x - cam, p.y, p.w, p.h);
      ctx.globalAlpha = 1;
    }

    for (const h of level.hazards) {
      ctx.fillStyle = 'rgba(200, 50, 60, 0.55)';
      ctx.fillRect(h.x - cam, h.y, h.w, h.h);
    }

    const t = Date.now() / 1000;
    for (const enemy of level.enemies || []) {
      const bob = Math.sin(t * 6 + enemy.x * 0.01) * 2;
      ctx.fillStyle = enemy.color || '#d06a58';
      ctx.fillRect(enemy.x - cam, enemy.y + bob, enemy.w, enemy.h);
      ctx.fillStyle = 'rgba(255,255,255,0.35)';
      ctx.fillRect(enemy.x - cam + 6, enemy.y + bob + 4, 5, 3);
      ctx.fillRect(enemy.x - cam + enemy.w - 11, enemy.y + bob + 4, 5, 3);
    }

    for (const c of level.collectibles) {
      if (c.taken) continue;
      const bob = Math.sin(t * 4 + c.x * 0.02) * 3;
      ctx.fillStyle = '#f0d848';
      ctx.beginPath();
      ctx.arc(c.x - cam, c.y + bob, 10, 0, Math.PI * 2);
      ctx.fill();
      ctx.strokeStyle = 'rgba(255,255,255,0.35)';
      ctx.lineWidth = 1;
      ctx.stroke();
    }

    for (const g of level.growth || []) {
      if (g.taken) continue;
      const bob = Math.sin(t * 5 + g.x * 0.02) * 4;
      const pulse = 11 + Math.sin(t * 6) * 1.5;
      ctx.fillStyle = '#f078a8';
      ctx.beginPath();
      ctx.arc(g.x - cam, g.y + bob, pulse, 0, Math.PI * 2);
      ctx.fill();
    }

    for (const p of level.powerups || []) {
      if (p.taken) continue;
      const bob = Math.sin(t * 5.5 + p.x * 0.03) * 3;
      ctx.fillStyle = p.kind === 'shield' ? '#8fd4ff' : p.kind === 'jump' ? '#f0a8ff' : '#8ff0b8';
      ctx.fillRect(p.x - cam - 10, p.y + bob - 8, 20, 16);
      ctx.fillStyle = 'rgba(255,255,255,0.35)';
      ctx.fillRect(p.x - cam - 5, p.y + bob - 4, 10, 3);
    }

    for (const cp of level.checkpoints || []) {
      ctx.fillStyle = 'rgba(100, 200, 255, 0.35)';
      ctx.fillRect(cp.x - cam - 8, cp.y - 24, 16, 32);
    }

    const f = level.finish;
    const exitReady = this._canCompleteLevel();
    const pulse = exitReady ? 0.45 + Math.sin(Date.now() / 280) * 0.15 : 0.2;
    ctx.fillStyle = `rgba(110, 240, 200, ${pulse})`;
    ctx.fillRect(f.x - cam, f.y, f.w, f.h);
    ctx.strokeStyle = exitReady ? '#6ef0c8' : '#4a8a78';
    ctx.lineWidth = exitReady ? 3 : 2;
    ctx.strokeRect(f.x - cam, f.y, f.w, f.h);

    this.effects.draw(ctx, cam);
    this.player.draw(ctx, cam);
  }

  loop(time) {
    const dt = Math.min(0.033, (time - this.lastTime) / 1000 || 0.016);
    this.lastTime = time;
    this.update(dt);
    this.draw();
    requestAnimationFrame((t) => this.loop(t));
  }

  start() {
    requestAnimationFrame((t) => this.loop(t));
  }
}
