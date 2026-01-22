/**
 * Utilitários gerais usados pelo projeto.
 * Mantidos pequenos e, quando possível, puros para facilitar testes.
 */
/**
 * Limita um valor entre `min` e `max` (inclusivo).
 */
export const clamp = (v, min, max) => Math.max(min, Math.min(max, v));

/**
 * Distância ao quadrado entre dois pontos — evita `sqrt` quando só se comparam distâncias.
 */
export const dist2 = (ax, ay, bx, by) => {
  const dx = ax - bx;
  const dy = ay - by;
  return dx * dx + dy * dy;
};

/**
 * Pequena função de asserção que lança uma exceção legível quando a condição falha.
 */
export function invariant(cond, msg) {
  if (!cond) throw new Error(`[Invariant] ${msg}`);
}

/**
 * Wrappers de seleção do DOM (conveniência mínima para manter o código enxuto).
 */
export const $ = (sel, root = document) => root.querySelector(sel);
export const $$ = (sel, root = document) => Array.from(root.querySelectorAll(sel));
