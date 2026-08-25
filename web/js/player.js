export class Player {
  constructor() {
    this.reset(80, 380);
    this.stage = null;
    this.baseStage = null;
    this.grounded = false;
    this.jumpsUsed = 0;
    this.dashCooldown = 0;
    this.role = null;
    this.checkpoint = { x: 80, y: 380 };
    this.coyoteTime = 0;
    this.invulnTime = 0;
    this.jumpDenied = false;
    this.justRespawned = false;
    this.justCheckpoint = false;
    this.justJumped = false;
    this.justDoubleJumped = false;
    this.justDashed = false;
    this.justPowerup = false;
    this.powerupKind = null;
    this.ducking = false;
  }

  reset(x, y) {
    this.x = x;
    this.y = y;
    this.vx = 0;
    this.vy = 0;
    this.jumpsUsed = 0;
    this.dashCooldown = 0;
    this.role = null;
    this.justJumped = false;
    this.justDoubleJumped = false;
    this.justDashed = false;
    this.justPowerup = false;
    this.powerupKind = null;
    this.ducking = false;
    if (this.baseStage) this._applyBaseStats();
  }

  applyStage(stage) {
    this.baseStage = { ...stage };
    this.stage = { ...stage };
  }

  _applyBaseStats() {
    Object.assign(this.stage, this.baseStage);
  }

  applyGrowth(speed, jump) {
    this.stage.moveSpeed += speed;
    this.stage.jumpForce += jump;
    this.stage.canJump = this.stage.jumpForce > 0;
  }

  applyPowerup(powerup) {
    if (!powerup) return;
    if (powerup.kind === 'speed') {
      this.applyGrowth(powerup.speed || 18, 0);
    } else if (powerup.kind === 'jump') {
      this.applyGrowth(0, powerup.jump || 28);
    } else if (powerup.kind === 'shield') {
      this.invulnTime = Math.max(this.invulnTime, powerup.duration || 2.5);
    }
    this.justPowerup = true;
    this.powerupKind = powerup.kind || 'boost';
  }

  _currentRadius() {
    const base = this.stage?.radius || 16;
    return this.ducking ? Math.max(10, base * 0.72) : base;
  }

  setCheckpoint(x, y) {
    this.checkpoint = { x, y };
  }

  respawn(level) {
    this.reset(this.checkpoint.x, this.checkpoint.y);
    this.snapToGround(level);
    this.invulnTime = 0.8;
    this.justRespawned = true;
  }

  get dashReady() {
    return this.dashCooldown <= 0 ? 1 : 1 - this.dashCooldown / 0.5;
  }

  update(dt, input, level) {
    if (!this.stage) return;

    input.tick(dt);
    this.jumpDenied = false;
    this.justJumped = false;
    this.justDoubleJumped = false;
    this.justDashed = false;
    this.justPowerup = false;
    this.powerupKind = null;

    this.ducking = input.isDuckHeld() && this.grounded;

    const s = this.stage;
    const move = input.getHorizontal();
    const canGroundJump = this.grounded || this.coyoteTime > 0;
    const r = this._currentRadius();

    if (this.dashCooldown > 0) this.dashCooldown -= dt;
    if (this.invulnTime > 0) this.invulnTime -= dt;

    if (input.wantsJump()) {
      if (canGroundJump && s.canJump && s.jumpForce > 0) {
        this.vy = -s.jumpForce;
        this.jumpsUsed = 1;
        this.grounded = false;
        this.coyoteTime = 0;
        this.justJumped = true;
        input.clearJump();
      } else if (s.canDoubleJump && this.jumpsUsed === 1 && !this.grounded) {
        this.vy = -s.jumpForce;
        this.jumpsUsed = 2;
        this.justDoubleJumped = true;
        input.clearJump();
      } else if (!s.canJump || s.jumpForce <= 0) {
        this.jumpDenied = true;
      }
    }

    if (s.canDash && input.consumeDash() && this.dashCooldown <= 0) {
      const dir = move !== 0 ? Math.sign(move) : (this.vx >= 0 ? 1 : -1);
      this.vx = dir * 420;
      this.dashCooldown = 0.5;
      this.justDashed = true;
    }

    const accel = s.floatMode ? 520 : 680;
    const targetVx = move * s.moveSpeed * (this.ducking ? 0.78 : 1);
    const control = this.grounded ? 1 : s.airControl;
    this.vx += (targetVx - this.vx) * Math.min(1, accel * control * dt / 120);

    if (s.floatMode && !this.grounded) {
      this.vy *= 1 - dt * 0.55;
      if (this.vy > 95) this.vy = 95;
    }

    const inWind = (level.winds || (level.wind ? [level.wind] : [])).some((w) =>
      this.x > w.x && this.x < w.x + w.w && this.y > w.y && this.y < w.y + w.h);

    for (const w of level.winds || (level.wind ? [level.wind] : [])) {
      if (this.x > w.x && this.x < w.x + w.w && this.y > w.y && this.y < w.y + w.h) {
        this.vx += (w.fx || 0) * dt;
        if (w.fy && !this.grounded) this.vy += w.fy * dt;
      }
    }

    if (s.floatMode && !this.grounded && inWind) {
      this.vy -= 320 * dt;
    }

    this.vy += s.gravity * dt;
    this.x += this.vx * dt;
    this.y += this.vy * dt;

    const pad = r + 4;
    const maxX = (level.worldWidth || 960) - pad;
    if (this.x < pad) { this.x = pad; if (this.vx < 0) this.vx = 0; }
    if (this.x > maxX) { this.x = maxX; if (this.vx > 0) this.vx = 0; }

    this.grounded = false;

    for (const pad of level.rolePads || []) {
      if (this._hitsBox(pad, r)) this.role = pad.role;
    }

    for (const p of level.platforms) {
      if (p.gateRole && this.role === p.gateRole) continue;
      if (this._resolvePlatform(p, r)) {
        this.grounded = true;
        this.jumpsUsed = 0;
      }
    }

    if (this.grounded) {
      this.coyoteTime = 0.14;
    } else {
      this.coyoteTime = Math.max(0, this.coyoteTime - dt);
    }

    if (this.invulnTime <= 0) {
      for (const h of level.hazards) {
        if (h.deadly !== false && this._hitsBox(h, r)) { this.respawn(level); return; }
      }
      if (this.y > 580) this.respawn(level);
    }

    for (const c of level.collectibles) {
      if (!c.taken && Math.hypot(this.x - c.x, this.y - c.y) < r + 12) c.taken = true;
    }

    for (const p of level.powerups || []) {
      if (!p.taken && Math.hypot(this.x - p.x, this.y - p.y) < r + 12) {
        p.taken = true;
        this.applyPowerup(p);
      }
    }

    for (const g of level.growth || []) {
      if (!g.taken && Math.hypot(this.x - g.x, this.y - g.y) < r + 14) {
        g.taken = true;
        this.applyGrowth(g.speed, g.jump);
      }
    }

    for (const cp of level.checkpoints || []) {
      if (Math.hypot(this.x - cp.x, this.y - cp.y) < 40) {
        const nx = cp.x;
        const ny = cp.y;
        if (this.checkpoint.x !== nx || this.checkpoint.y !== ny) {
          this.setCheckpoint(nx, ny);
          this.snapToGround(level);
          this.justCheckpoint = true;
        }
      }
    }
  }

  _resolvePlatform(p, r) {
    const px = this.x, py = this.y;

    // Snap onto platform top when close (prevents one-frame fall-through).
    const snapDown = 10;
    if (this.vy >= 0 &&
        px + r > p.x && px - r < p.x + p.w &&
        py + r >= p.y && py + r <= p.y + snapDown) {
      this.y = p.y - r;
      this.vy = 0;
      return true;
    }

    if (px + r < p.x || px - r > p.x + p.w || py + r < p.y || py - r > p.y + p.h) return false;

    const overlapL = px + r - p.x, overlapR = p.x + p.w - (px - r);
    const overlapT = py + r - p.y, overlapB = p.y + p.h - (py - r);
    const min = Math.min(overlapL, overlapR, overlapT, overlapB);

    if (min === overlapT && this.vy >= 0) { this.y = p.y - r; this.vy = 0; return true; }
    if (min === overlapB && this.vy < 0) { this.y = p.y + p.h + r; this.vy = 0; }
    else if (min === overlapL) { this.x = p.x - r; this.vx = 0; }
    else if (min === overlapR) { this.x = p.x + p.w + r; this.vx = 0; }
    return false;
  }

  _hitsBox(box, r) {
    return this.x + r > box.x && this.x - r < box.x + (box.w || 0) &&
           this.y + r > box.y && this.y - r < box.y + (box.h || 0);
  }

  hitsFinish(level) {
    const f = level.finish, r = this._currentRadius();
    return this.x + r > f.x && this.x - r < f.x + f.w && this.y + r > f.y && this.y - r < f.y + f.h;
  }

  snapToGround(level) {
    const r = this._currentRadius();
    let bestY = null;
    for (const p of level.platforms) {
      if (p.gateRole) continue;
      if (this.x + r > p.x && this.x - r < p.x + p.w) {
        const surface = p.y - r;
        if (bestY === null || surface < bestY) bestY = surface;
      }
    }
    if (bestY !== null) {
      this.y = bestY;
      this.grounded = true;
      this.coyoteTime = 0.14;
    }
  }

  draw(ctx, camX) {
    const s = this.stage;
    const x = this.x - camX, y = this.y;
    const r = this._currentRadius();
    if (this.invulnTime > 0 && Math.floor(Date.now() / 100) % 2 === 0) {
      ctx.globalAlpha = 0.45;
    }
    ctx.fillStyle = s.color;
    ctx.beginPath();
    if (this.ducking) {
      ctx.ellipse(x, y + r * 0.18, r, r * 0.68, 0, 0, Math.PI * 2);
    } else {
      ctx.arc(x, y, r, 0, Math.PI * 2);
    }
    ctx.fill();
    ctx.strokeStyle = 'rgba(255,255,255,0.4)';
    ctx.lineWidth = 2;
    ctx.stroke();
    ctx.globalAlpha = 1;
    if (this.role) {
      ctx.fillStyle = this.role === 'nerve' ? '#88aaff' : '#ff8866';
      ctx.beginPath();
      ctx.arc(x, y - s.radius - 6, 4, 0, Math.PI * 2);
      ctx.fill();
    }
  }
}
