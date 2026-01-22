/**
 * Entrada principal da aplicação.
 * - Apenas inicializa a UI quando o DOM estiver pronto.
 */
import './utils.js';
import { initUI } from './ui.js';

window.addEventListener('DOMContentLoaded', () => {
  initUI();
});
