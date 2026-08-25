export class Input {
  constructor() {
    this.keys = {};
    this.moveX = 0;
    this.jumpBuffer = 0;
    this.dashPressed = false;
    this.duckHeld = false;
    this.pausePressed = false;

    window.addEventListener('keydown', (e) => {
      if (['Space', 'ArrowUp', 'ArrowDown', 'ArrowLeft', 'ArrowRight', 'KeyS'].includes(e.code)) {
        e.preventDefault();
      }
      if (!this.keys[e.code]) {
        if (e.code === 'Space') this.queueJump();
        if (e.code === 'ShiftLeft' || e.code === 'ShiftRight') this.dashPressed = true;
        if (e.code === 'Escape') this.pausePressed = true;
      }
      if (e.code === 'ArrowDown' || e.code === 'KeyS') this.duckHeld = true;
      this.keys[e.code] = true;
    });
    window.addEventListener('keyup', (e) => {
      if (e.code === 'ArrowDown' || e.code === 'KeyS') this.duckHeld = false;
      this.keys[e.code] = false;
    });

    this._setupJoystick();
    this._setupButtons();
  }

  _setupJoystick() {
    const zone = document.getElementById('joystick');
    const knob = document.getElementById('joystick-knob');
    if (!zone || !knob) return;

    let active = false;
    let startX = 0;
    let startY = 0;
    const maxDist = 36;

    const onStart = (x, y) => {
      active = true;
      const rect = zone.getBoundingClientRect();
      startX = rect.left + rect.width / 2;
      startY = rect.top + rect.height / 2;
      this._moveKnob(x, y, startX, startY, knob, maxDist);
    };

    const onMove = (x, y) => {
      if (!active) return;
      this._moveKnob(x, y, startX, startY, knob, maxDist);
    };

    const onEnd = () => {
      active = false;
      this.moveX = 0;
      knob.style.transform = 'translate(0px, 0px)';
    };

    zone.addEventListener('touchstart', (e) => {
      e.preventDefault();
      onStart(e.touches[0].clientX, e.touches[0].clientY);
    }, { passive: false });
    zone.addEventListener('touchmove', (e) => {
      e.preventDefault();
      onMove(e.touches[0].clientX, e.touches[0].clientY);
    }, { passive: false });
    zone.addEventListener('touchend', onEnd);
    zone.addEventListener('mousedown', (e) => onStart(e.clientX, e.clientY));
    window.addEventListener('mousemove', (e) => { if (active) onMove(e.clientX, e.clientY); });
    window.addEventListener('mouseup', onEnd);
  }

  _moveKnob(x, y, cx, cy, knob, maxDist) {
    let dx = x - cx;
    let dy = y - cy;
    const dist = Math.hypot(dx, dy);
    if (dist > maxDist) {
      dx = (dx / dist) * maxDist;
      dy = (dy / dist) * maxDist;
    }
    knob.style.transform = `translate(${dx}px, ${dy}px)`;
    this.moveX = dx / maxDist;
  }

  _setupButtons() {
    const jump = document.getElementById('btn-jump');
    const dash = document.getElementById('btn-dash');
    const duck = document.getElementById('btn-duck');
    const queueJump = (e) => {
      e.preventDefault();
      e.stopPropagation();
      this.queueJump();
    };
    if (jump) {
      jump.addEventListener('pointerdown', queueJump);
      jump.addEventListener('click', queueJump);
    }
    if (dash) {
      const queueDash = (e) => {
        e.preventDefault();
        e.stopPropagation();
        this.dashPressed = true;
      };
      dash.addEventListener('pointerdown', queueDash);
      dash.addEventListener('click', queueDash);
    }
    if (duck) {
      const setDuck = (pressed) => (e) => {
        e.preventDefault();
        e.stopPropagation();
        this.duckHeld = pressed;
      };
      duck.addEventListener('pointerdown', setDuck(true));
      duck.addEventListener('pointerup', setDuck(false));
      duck.addEventListener('pointercancel', setDuck(false));
      duck.addEventListener('pointerleave', setDuck(false));
    }
  }

  queueJump() {
    this.jumpBuffer = 0.18;
  }

  tick(dt) {
    if (this.jumpBuffer > 0) this.jumpBuffer -= dt;
  }

  getHorizontal() {
    let x = this.moveX;
    if (this.keys['KeyA'] || this.keys['ArrowLeft']) x = -1;
    if (this.keys['KeyD'] || this.keys['ArrowRight']) x = 1;
    return Math.max(-1, Math.min(1, x));
  }

  wantsJump() {
    return this.jumpBuffer > 0;
  }

  clearJump() {
    this.jumpBuffer = 0;
  }

  consumeDash() {
    const v = this.dashPressed;
    this.dashPressed = false;
    return v;
  }

  consumePause() {
    const v = this.pausePressed;
    this.pausePressed = false;
    return v;
  }

  isDuckHeld() {
    return this.duckHeld;
  }
}
