/**
 * Núcleo do engine: orquestração de entidades, waves e do loop principal.
 * Este módulo atua como um orquestrador leve — as entidades são responsáveis pelo
 * seu próprio DOM e pela lógica de atualização.
 */
import { CONFIG } from './config.js';
import Enemy from './entities/enemy.js';
import Tower from './entities/tower.js';
import Projectile from './entities/projectile.js';
import { pointInRadius } from './collision.js';
import { buildPathCells } from './path.js';

export const State = {
  phase: 'IDLE',
  wave: 1,
  lives: CONFIG.economy.startLives,
  gold: CONFIG.economy.startGold,
  selectedTowerType: 'snipper',

  enemies: [],
  towers: [],
  projectiles: [],

  toSpawn: 0,
  spawned: 0,
  spawnTimerMs: 0,
  interWaveTimerMs: 0,

  lastTs: performance.now(),
  paused: false,
  nextId: 1,
};

let WAYPOINTS = [];
let domLayers = null;
let rafId = null;

function cellsLine(c1, r1, c2, r2) {
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

// Construção do path movida para src/path.js (helpers puros, testáveis)

export function initEngine(layers) {
  domLayers = layers;
  WAYPOINTS = buildPathCells();
  State.enemies = [];
  State.towers = [];
  State.projectiles = [];
}

export function startWave() {
  if (State.phase !== 'IDLE') return;
  const count = CONFIG.wave.baseEnemies + (State.wave - 1) * CONFIG.wave.growthPerWave;
  State.toSpawn = count;
  State.spawned = 0;
  State.spawnTimerMs = 0;
  State.phase = 'SPAWNING';
}

export function placeTowerAt(center, type) {
  if (!CONFIG.towers[type]) return false;
  const cfg = CONFIG.towers[type];
  if (State.gold < cfg.cost) return false;
  State.gold -= cfg.cost;
  const id = State.nextId++;
  const t = new Tower({ id, x: center.x, y: center.y, cfg, layerEl: domLayers.towersLayer, onSpawnProjectile: spawnProjectileFromTower });
  State.towers.push(t);
  return true;
}

function spawnProjectileFromTower({ x, y, target, cfg }) {
  const id = State.nextId++;
  const p = new Projectile({ id, x, y, target, speed: cfg.projectileSpeedPxS, damage: cfg.damage, layerEl: domLayers.projectilesLayer, cfg: CONFIG, onHit: onProjectileHit });
  State.projectiles.push(p);
}

function onProjectileHit(projectile, enemy) {
  State.gold += 0;
}

function spawnEnemy(wave) {
  const id = State.nextId++;
  const e = new Enemy({ id, waypoints: WAYPOINTS, wave, cfg: CONFIG.enemy, layerEl: domLayers.enemiesLayer, onLeak: onEnemyLeak, onDeath: onEnemyDeath });
  State.enemies.push(e);
}

function onEnemyLeak(enemy) {
  if (enemy.alive) {
    enemy.alive = false;
    State.lives = Math.max(0, State.lives - CONFIG.enemy.leakDamage);
  }
}

function onEnemyDeath(enemy) {
  if (!enemy._rewarded) {
    enemy._rewarded = true;
    State.gold += CONFIG.enemy.rewardGold;
  }
}

function cleanup() {
  State.enemies = State.enemies.filter(e => {
    const keep = (e.alive && !e.leaked) || (!e.alive && e.hp > 0);
    if (!keep) e.destroy();
    return keep;
  });
  State.projectiles = State.projectiles.filter(p => {
    if (!p.alive) p.destroy();
    return p.alive;
  });
}

function updateWave(dtMs) {
  switch (State.phase) {
    case 'SPAWNING':
      State.spawnTimerMs += dtMs;
      while (State.spawnTimerMs >= CONFIG.wave.spawnIntervalMs && State.spawned < State.toSpawn) {
        State.spawnTimerMs -= CONFIG.wave.spawnIntervalMs;
        spawnEnemy(State.wave);
        State.spawned++;
      }
      if (State.spawned >= State.toSpawn) State.phase = 'CLEARING';
      break;
    case 'CLEARING':
      const remaining = State.enemies.some(e => e.alive && !e.leaked);
      const projectilesAlive = State.projectiles.some(p => p.alive);
      if (!remaining && !projectilesAlive) {
        State.phase = 'INTERWAVE';
        State.interWaveTimerMs = 0;
      }
      break;
    case 'INTERWAVE':
      State.interWaveTimerMs += dtMs;
      if (State.interWaveTimerMs >= CONFIG.wave.interWaveDelayMs) {
        State.wave++;
        State.phase = 'IDLE';
      }
      break;
    default:
      break;
  }
}

export function tickLoop(ts) {
  const rawDt = (ts - State.lastTs) / 1000;
  State.lastTs = ts;
  if (State.paused || State.phase === 'GAME_OVER') {
    rafId = requestAnimationFrame(tickLoop);
    return;
  }
  const dt = Math.max(0, Math.min(rawDt, CONFIG.engine.maxDeltaS));
  const dtMs = dt * 1000;

  updateWave(dtMs);

  for (const e of State.enemies) e.update(dt, WAYPOINTS);
  for (const t of State.towers) t.update(dt, State.enemies);
  for (const p of State.projectiles) p.update(dt, CONFIG, pointInRadius);

  cleanup();

  if (State.lives <= 0 && State.phase !== 'GAME_OVER') {
    State.phase = 'GAME_OVER';
  }

  rafId = requestAnimationFrame(tickLoop);
}

export function bootEngine() {
  State.lastTs = performance.now();
  rafId = requestAnimationFrame(tickLoop);
}

export function togglePause() {
  State.paused = !State.paused;
}
