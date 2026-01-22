/**
 * UI glue: ligação com o DOM e lógica de apresentação leve.
 * Este arquivo deve apenas manipular DOM e entrada do usuário; o estado do jogo
 * é alterado via API do engine.
 */
import { CONFIG } from './config.js';
import { $, $$ } from './utils.js';
import { initEngine, startWave, placeTowerAt, bootEngine, State, togglePause } from './engine.js';

// Note: imported names adjusted; helper wrappers below to avoid name clashes
const $sel = (s, r = document) => r.querySelector(s);
const $$sel = (s, r = document) => Array.from(r.querySelectorAll(s));

export function initUI() {
  const dom = {
    map: $sel('#' + CONFIG.dom.ids.map),
    stage: $sel('#' + CONFIG.dom.ids.stage),
    vida: $sel('#' + CONFIG.dom.ids.vida),
    ouro: $sel('#' + CONFIG.dom.ids.ouro),
    wave: $sel('#' + CONFIG.dom.ids.wave),
    enemiesLeft: $sel('#' + CONFIG.dom.ids.enemiesLeft),
    startWave: $sel('#' + CONFIG.dom.ids.startWave),
    pause: $sel('#' + CONFIG.dom.ids.pause),
    status: $sel('#' + CONFIG.dom.ids.status),
    selectedTower: $sel('#' + CONFIG.dom.ids.selectedTower),

    enemiesLayer: $sel('.' + CONFIG.dom.classes.enemiesLayer),
    towersLayer: $sel('.' + CONFIG.dom.classes.towersLayer),
    projectilesLayer: $sel('.' + CONFIG.dom.classes.projectilesLayer),

    shopItems: $$sel('.' + CONFIG.dom.classes.towerItem),
  };

  if (!dom.map || !dom.stage) throw new Error('Missing DOM: #map and #stage required');

  document.documentElement.style.setProperty('--cell', `${CONFIG.grid.cellSize}px`);
  document.documentElement.style.setProperty('--cols', `${CONFIG.grid.cols}`);
  document.documentElement.style.setProperty('--rows', `${CONFIG.grid.rows}`);

  buildGrid(dom.map);

  initEngine({ enemiesLayer: dom.enemiesLayer, towersLayer: dom.towersLayer, projectilesLayer: dom.projectilesLayer });

  dom.startWave.addEventListener('click', () => {
    startWave();
    syncUI(dom);
  });

  dom.pause.addEventListener('click', () => {
    togglePause();
    dom.pause.textContent = State.paused ? 'Continuar' : 'Pausar';
    setStatus(dom, State.paused ? 'Pausado.' : 'Rodando.');
  });

  for (const item of dom.shopItems) {
    item.addEventListener('click', () => {
      const type = item.dataset.type || item.getAttribute('datatype') || item.getAttribute('data-type') || '';
      if (!type || !CONFIG.towers[type]) return setStatus(dom, 'Item inválido');
      State.selectedTowerType = type;
      markSelectedShopItem(dom, type);
      if (dom.selectedTower) dom.selectedTower.textContent = `Selecionada: ${CONFIG.towers[type].label}`;
    });
  }

  dom.map.addEventListener('click', (ev) => {
    const cell = ev.target.closest('.cell');
    if (!cell) return;
    const c = Number(cell.dataset.c);
    const r = Number(cell.dataset.r);
    const key = `${c},${r}`;
    // Check path set by re-using the same path calc
    if (isPathCell(c, r)) return setStatus(dom, 'Não pode colocar torre no caminho.');
    const center = cellCenterPx(c, r);
    const occupied = State.towers.some(t => Math.hypot(t.x - center.x, t.y - center.y) < 1);
    if (occupied) return setStatus(dom, 'Já existe uma torre aqui.');
    const ok = placeTowerAt(center, State.selectedTowerType);
    if (!ok) return setStatus(dom, 'Ouro insuficiente ou tipo inválido.');
    syncUI(dom);
    setStatus(dom, 'Torre colocada.');
  });

  syncUI(dom);
  setStatus(dom, 'Pronto. Inicie uma wave.');

  bootEngine();

  // tick-based UI sync loop
  (function uiLoop() {
    syncUI(dom);
    requestAnimationFrame(uiLoop);
  })();
}

function setStatus(dom, msg) {
  if (dom.status) dom.status.textContent = msg;
}

function syncUI(dom) {
  dom.vida.textContent = String(State.lives);
  dom.ouro.textContent = String(State.gold);
  dom.wave.textContent = String(State.wave);
  const aliveOrLeaking = State.enemies.filter(e => e.alive && !e.leaked).length;
  dom.enemiesLeft.textContent = String(aliveOrLeaking);
  dom.startWave.disabled = !(State.phase === 'IDLE');
}

function markSelectedShopItem(dom, type) {
  for (const item of dom.shopItems) {
    const t = item.dataset.type || item.getAttribute('datatype') || item.getAttribute('data-type') || '';
    item.classList.toggle('selected', t === type);
  }
}

function buildGrid(mapEl) {
  mapEl.innerHTML = '';
  mapEl.style.gridTemplateColumns = `repeat(${CONFIG.grid.cols}, ${CONFIG.grid.cellSize}px)`;
  mapEl.style.gridTemplateRows = `repeat(${CONFIG.grid.rows}, ${CONFIG.grid.cellSize}px)`;
  for (let r = 0; r < CONFIG.grid.rows; r++) {
    for (let c = 0; c < CONFIG.grid.cols; c++) {
      const cell = document.createElement('div');
      cell.className = 'cell';
      cell.dataset.c = String(c);
      cell.dataset.r = String(r);
      if (isPathCell(c, r)) cell.classList.add('path');
      mapEl.appendChild(cell);
    }
  }
}

function isPathCell(c, r) {
  const PATH_CELLS = [
    ...cellsLine(0, 2, 6, 2),
    ...cellsLine(6, 3, 6, 6),
    ...cellsLine(7, 6, 12, 6),
    ...cellsLine(12, 5, 12, 1),
    ...cellsLine(13, 1, 15, 1),
  ];
  return new Set(PATH_CELLS.map(p => `${p.c},${p.r}`)).has(`${c},${r}`);
}

function cellCenterPx(c, r) {
  return {
    x: c * CONFIG.grid.cellSize + CONFIG.grid.cellSize / 2,
    y: r * CONFIG.grid.cellSize + CONFIG.grid.cellSize / 2,
  };
}

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
