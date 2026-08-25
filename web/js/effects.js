/** Lightweight pickup / checkpoint sparkles — visual only. */
export class Effects {
  constructor() {
    this.items = [];
  }

  burst(x, y, color, count = 8) {
    for (let i = 0; i < count; i++) {
      const angle = (Math.PI * 2 * i) / count + Math.random() * 0.4;
      const speed = 60 + Math.random() * 120;
      this.items.push({
        x,
        y,
        vx: Math.cos(angle) * speed,
        vy: Math.sin(angle) * speed - 40,
        life: 0.45 + Math.random() * 0.2,
        maxLife: 0.65,
        color,
        size: 3 + Math.random() * 3,
      });
    }
  }

  update(dt) {
    for (const p of this.items) {
      p.x += p.vx * dt;
      p.y += p.vy * dt;
      p.vy += 180 * dt;
      p.life -= dt;
    }
    this.items = this.items.filter((p) => p.life > 0);
  }

  draw(ctx, camX) {
    for (const p of this.items) {
      const t = p.life / p.maxLife;
      ctx.globalAlpha = Math.max(0, t);
      ctx.fillStyle = p.color;
      ctx.beginPath();
      ctx.arc(p.x - camX, p.y, p.size * t, 0, Math.PI * 2);
      ctx.fill();
    }
    ctx.globalAlpha = 1;
  }
}
