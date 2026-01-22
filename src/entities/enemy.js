import { clamp } from '../utils.js';

/**
 * Entidade Enemy
 * - Gerencia estado, movimento e render do inimigo.
 * - Recebe callbacks `onLeak` e `onDeath` para que o engine aplique efeitos colaterais (vida, recompensa).
 */
export default class Enemy {
  constructor({ id, waypoints, wave, cfg, layerEl, onLeak, onDeath }) {
    this.id = id;
    this.wp = 0;

    const start = waypoints[0];
    this.x = start.x;
    this.y = start.y;

    const hpMul = 1 + (wave - 1) * cfg.hpGrowth;
    const spMul = 1 + (wave - 1) * cfg.speedGrowth;

    this.maxHp = Math.round(cfg.baseHp * hpMul);
    this.hp = this.maxHp;
    this.speed = cfg.baseSpeed * spMul;

    this.alive = true;
    this.leaked = false;

    this._onLeak = onLeak;
    this._onDeath = onDeath;

    this.el = document.createElement('div');
    this.el.className = 'enemy';
    this.el.style.transform = `translate(${this.x}px, ${this.y}px) translate(-50%, -50%)`;

    const bar = document.createElement('div');
    bar.className = 'hpbar';
    const fill = document.createElement('div');
    fill.className = 'hpfill';
    bar.appendChild(fill);
    this.el.appendChild(bar);
    this.hpFill = fill;

    layerEl.appendChild(this.el);
    this.render();
  }

  render() {
    this.el.style.transform = `translate(${this.x}px, ${this.y}px) translate(-50%, -50%)`;
    const ratio = clamp(this.hp / this.maxHp, 0, 1);
    this.hpFill.style.width = `${Math.round(ratio * 100)}%`;
    this.hpFill.style.opacity = `${0.55 + 0.45 * ratio}`;
  }

  update(dt, waypoints) {
    if (!this.alive || this.leaked) return;

    if (this.wp >= waypoints.length - 1) {
      this.leaked = true;
      this._onLeak && this._onLeak(this);
      return;
    }

    const t = waypoints[this.wp + 1];
    const dx = t.x - this.x;
    const dy = t.y - this.y;
    const d = Math.hypot(dx, dy);
    const step = this.speed * dt;

    if (step >= d) {
      this.x = t.x;
      this.y = t.y;
      this.wp++;
    } else {
      this.x += (dx / d) * step;
      this.y += (dy / d) * step;
    }

    this.render();
  }

  hit(dmg) {
    if (!this.alive) return;
    this.hp -= dmg;
    if (this.hp <= 0) {
      this.hp = 0;
      this.alive = false;
      this._onDeath && this._onDeath(this);
    }
    this.render();
  }

  destroy() {
    this.el.remove();
  }
}
