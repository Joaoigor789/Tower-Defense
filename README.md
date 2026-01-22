# Tower-Defense


Distribuição de Responsabilidades

. Italo - Frontend Dinâmico & Game Logic (JavaScript Puro/css)

- Toda a lógica do jogo em game.js
- Sistema de colisão
- Movimento dos inimigos
- Sistema de tiros das torres
- Controle de waves e dificuldade
- Animações com JavaScript
- Controle de estado do jogo
- Integração com o HTML do João


---------------------------------------

. Pamela - Backend & API

- Servidor Node.js/Express
- Sistema de ranking/placares
- Salvar progresso dos jogadores
- Multiplayer (se quiserem)
- Banco de dados (MongoDB/MySQL)
- Autenticação de usuários
- API para carregar fases

---

## Como rodar localmente (desenvolvimento)

Siga estes passos para preparar o ambiente e executar o projeto localmente:

1. Instale o Node.js (v16+ recomendado) e o npm.
2. No terminal, na raiz do projeto, instale dependências:

```bash
npm install
```

3. Inicie um servidor local (abre a pasta como site estático):

```bash
npm run serve
```

O projeto ficará disponível em `http://127.0.0.1:8080` por padrão.

## Testes automatizados

Executamos testes com Vitest. Para rodar os testes:

```bash
npm test
```

Os testes de unidade ficam em `test/` e cobrem utilitários puros (fácil de estender).

## Estrutura do projeto (resumido)

- `index.html` — entrada da aplicação (carrega `src/main.js` como módulo).
- `styles.css` — estilos.
- `src/` — código fonte modularizado:
	- `src/main.js` — entrypoint que inicializa a UI.
	- `src/ui.js` — ligação com o DOM e handlers de input.
	- `src/engine.js` — orquestração do jogo (estado, loop, spawn).
	- `src/entities/` — implementações de `Enemy`, `Tower`, `Projectile`.
	- `src/utils.js`, `src/collision.js`, `src/path.js`, `src/config.js` — helpers e configuração.
- `test/` — testes unitários (Vitest).


