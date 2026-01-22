import { describe, it, expect } from 'vitest';
import { cellsLine, buildPathCells, pathSet } from '../src/path.js';

describe('path helpers', () => {
  it('creates straight line of cells', () => {
    const line = cellsLine(0,0, 0,2);
    expect(line).toEqual([{c:0,r:0},{c:0,r:1},{c:0,r:2}]);
  });

  it('builds waypoints and set', () => {
    const w = buildPathCells();
    expect(Array.isArray(w)).toBe(true);
    const s = pathSet();
    expect(s.size).toBeGreaterThan(0);
  });
});
