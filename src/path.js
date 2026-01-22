/**
 * Utilitários de caminho (path)
 * - Fornece helpers para construir células do caminho e centros de waypoints para a grid do jogo.
 * - Funções puras (sem DOM) para facilitar testes unitários.
 */
import { CONFIG } from './config.js';

export function cellsLine(c1, r1, c2, r2) {
  const out = [];
  const dc = Math.sign(c2 - c1);
  const dr = Math.sign(r2 - r1);
  let c = c1, r = r1;
  out.push({ c, r });
  while (c !== c2 || r !== r2) {
    if (c !== c2) c += dc;
    else if (r !== r2) r += dr;
    out.push({ c, r });
  }
  return out;
}

export function buildPathCells() {
  const PATH_CELLS = [
    ...cellsLine(0, 2, 6, 2),
    ...cellsLine(6, 3, 6, 6),
    ...cellsLine(7, 6, 12, 6),
    ...cellsLine(12, 5, 12, 1),
    ...cellsLine(13, 1, 15, 1),
  ];

  function cellCenterPx(c, r) {
    return {
      x: c * CONFIG.grid.cellSize + CONFIG.grid.cellSize / 2,
      y: r * CONFIG.grid.cellSize + CONFIG.grid.cellSize / 2,
    };
  }

  return PATH_CELLS.map(p => cellCenterPx(p.c, p.r));
}

/**
 * Retorna um Set com as posições do caminho no formato "c,r" para checagem rápida.
 */
export function pathSet() {
  const PATH_CELLS = [
    ...cellsLine(0, 2, 6, 2),
    ...cellsLine(6, 3, 6, 6),
    ...cellsLine(7, 6, 12, 6),
    ...cellsLine(12, 5, 12, 1),
    ...cellsLine(13, 1, 15, 1),
  ];
  return new Set(PATH_CELLS.map(p => `${p.c},${p.r}`));
}
