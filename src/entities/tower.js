import Projectile from './projectile.js';

/**
 * Entidade Tower
 * - Responsável por decidir alvos e solicitar a criação de projéteis via callback.
 * - Mantém cooldown local e não manipula diretamente a economia do jogo.
 */
export default class Tower {
  constructor({ id, x, y, cfg, layerEl, onSpawnProjectile }) {
    this.id = id;
    this.x = x;
    this.y = y;
    this.cfg = cfg;

    this.cooldown = 0;

    this.el = document.createElement('div');
    this.el.className = 'tower';
    this.el.style.transform = `translate(${this.x}px, ${this.y}px) translate(-50%, -50%)`;
    layerEl.appendChild(this.el);

    this._onSpawnProjectile = onSpawnProjectile;
  }

  update(dt, enemies) {
    this.cooldown = Math.max(0, this.cooldown - dt);

    let target = null;
    let best = Infinity;
    const r2 = this.cfg.rangePx * this.cfg.rangePx;

    for (const e of enemies) {
      if (!e.alive || e.leaked) continue;
      const dx = this.x - e.x;
      const dy = this.y - e.y;
      const d2v = dx * dx + dy * dy;
      if (d2v <= r2 && d2v < best) {
        best = d2v;
        target = e;
      }
    }

    if (!target) return;

    if (this.cooldown <= 0) {
      this._onSpawnProjectile && this._onSpawnProjectile({ x: this.x, y: this.y, target, cfg: this.cfg });
      this.cooldown = 1 / this.cfg.fireRate;
    }
  }
}
