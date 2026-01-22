import { describe, it, expect } from 'vitest';
import { clamp, dist2 } from '../src/utils.js';

describe('utils', () => {
  it('clamps values', () => {
    expect(clamp(5, 0, 10)).toBe(5);
    expect(clamp(-1, 0, 10)).toBe(0);
    expect(clamp(12, 0, 10)).toBe(10);
  });

  it('computes squared distance', () => {
    expect(dist2(0, 0, 3, 4)).toBe(25);
  });
});
