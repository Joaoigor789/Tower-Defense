/**
 * Configuração central do jogo.
 * Mantenha apenas dados estáticos aqui; ajuste de balanceamento deve ser feito via esses valores.
 */
export const CONFIG = Object.freeze({
  grid: { cols: 16, rows: 9, cellSize: 48 },
  economy: { startGold: 150, startLives: 100 },

  wave: {
    baseEnemies: 8,
    growthPerWave: 3,
    spawnIntervalMs: 550,
    interWaveDelayMs: 1200,
  },

  enemy: {
    size: 22,
    baseHp: 60,
    hpGrowth: 0.25,
    baseSpeed: 80,
    speedGrowth: 0.05,
    rewardGold: 12,
    leakDamage: 10,
  },

  towers: {
    snipper: {
      label: "Snipper",
      cost: 50,
      rangePx: 170,
      fireRate: 1.2,
      damage: 22,
      projectileSpeedPxS: 320,
    },
  },

  projectile: { size: 6, hitRadiusPx: 14, maxLifetimeS: 2.5 },

  engine: { maxDeltaS: 0.05 },

  dom: {
    ids: {
      map: "map",
      stage: "stage",
      vida: "vida",
      ouro: "ouro",
      wave: "wave",
      enemiesLeft: "enemies-left",
      startWave: "start-wave",
      pause: "pause",
      status: "status",
      selectedTower: "selected-tower",
    },
    classes: {
      enemiesLayer: "enemies-container",
      towersLayer: "towers-container",
      projectilesLayer: "projectiles-container",
      towerItem: "tower-item",
    },
  },
});
