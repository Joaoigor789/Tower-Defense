/**
 * Helpers de colisão (matemática pura) — sem dependência ao DOM para facilitar testes.
 */
import { dist2 } from './utils.js';

/**
 * Verifica colisão entre dois círculos (usa distância ao quadrado).
 */
export function circleCollision(ax, ay, ar, bx, by, br) {
  const r = ar + br;
  return dist2(ax, ay, bx, by) <= r * r;
}

/**
 * Verifica se um ponto está dentro de um raio (comparação por quadrado da distância).
 */
export function pointInRadius(px, py, cx, cy, radius) {
  return dist2(px, py, cx, cy) <= radius * radius;
}
