/**
 * Entidade Projectile
 * - Representa um projétil lançado por uma torre e contém sua própria representação DOM.
 * - Notifica via callback `onHit` quando acerta o alvo.
 */
export default class Projectile {
  constructor({ id, x, y, target, speed, damage, layerEl, cfg, onHit }) {
    this.id = id;
    this.x = x;
    this.y = y;
    this.target = target;

    this.speed = speed;
    this.damage = damage;

    this.alive = true;
    this.life = 0;

    this._onHit = onHit;

    this.el = document.createElement('div');
    this.el.className = 'projectile';
    this.el.style.transform = `translate(${this.x}px, ${this.y}px) translate(-50%, -50%)`;
    layerEl.appendChild(this.el);
  }

  render() {
    this.el.style.transform = `translate(${this.x}px, ${this.y}px) translate(-50%, -50%)`;
  }

  update(dt, config, pointInRadius) {
    if (!this.alive) return;

    this.life += dt;
    if (this.life > config.projectile.maxLifetimeS) {
      this.alive = false;
      return;
    }

    if (!this.target || !this.target.alive || this.target.leaked) {
      this.alive = false;
      return;
    }

    const tx = this.target.x;
    const ty = this.target.y;
    const dx = tx - this.x;
    const dy = ty - this.y;
    const d = Math.hypot(dx, dy);

    if (d <= config.projectile.hitRadiusPx) {
      this.target.hit(this.damage);
      this._onHit && this._onHit(this, this.target);
      this.alive = false;
      return;
    }

    const step = this.speed * dt;
    this.x += (dx / d) * step;
    this.y += (dy / d) * step;

    this.render();
  }

  destroy() {
    this.el.remove();
  }
}
