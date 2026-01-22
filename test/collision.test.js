import { describe, it, expect } from 'vitest';
import { circleCollision, pointInRadius } from '../src/collision.js';

describe('collision', () => {
  it('detects circle collisions', () => {
    expect(circleCollision(0,0,5, 8,0,4)).toBe(true);
    expect(circleCollision(0,0,2, 10,0,2)).toBe(false);
  });

  it('detects point in radius', () => {
    expect(pointInRadius(0,0, 3,4, 5)).toBe(true);
    expect(pointInRadius(0,0, 6,8, 5)).toBe(false);
  });
});
