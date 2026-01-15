# 📡 Tower Defense API - Documentação para Integração Frontend

**Backend desenvolvido por: Pamela Menezes**  
**Stack**: .NET 8, PostgreSQL, JWT, SignalR

---

## 🚀 Como Rodar o Backend

```bash
cd Backend.TowerDefense/TowerDefense.API
dotnet run
```

**API estará disponível em**: `http://localhost:5000`  
**Swagger (documentação interativa)**: `http://localhost:5000`  
**Health Check**: `http://localhost:5000/health`

---

## 🛡️ Health Check

**GET** `/health`

Verifica o status da API e conectividade com PostgreSQL.

**Resposta (200 OK)**: `Healthy`

**Uso**:
```bash
curl http://localhost:5000/health
```

---

## 🔐 Autenticação

Todos os endpoints (exceto `/auth/login` e `/auth/register`) requerem **JWT Token** no header:

```javascript
Authorization: Bearer {seu_token_jwt}
```

### 1. Registrar Novo Jogador

**POST** `/api/auth/register`

```json
{
  "username": "pamela",
  "email": "pamela@email.com",
  "password": "senha123",
  "confirmPassword": "senha123"
}
```

**Resposta (200 OK)**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "pamela",
  "email": "pamela@email.com",
  "highScore": 0,
  "expiresAt": "2026-01-16T16:00:00Z"
}
```

### 2. Login

**POST** `/api/auth/login`

```json
{
  "usernameOrEmail": "pamela",
  "password": "senha123"
}
```

**Resposta**: Mesma estrutura do registro.

---

## 🏰 Assets API (Torres)

### 3. Listar Todas as Torres

**GET** `/api/assets/towers`

**Resposta (200 OK)**:
```json
[
  {
    "name": "Archer Tower",
    "description": "Torre básica que atira flechas. Bom contra inimigos leves.",
    "damage": 10,
    "range": 5.0,
    "attackSpeed": 1.5,
    "cost": 100,
    "spriteUrl": "/assets/sprites/archer-tower.png",
    "attackSoundUrl": "/assets/sounds/arrow-shot.mp3",
    "towerType": "Physical",
    "damagePerSecond": 15.0
  },
  {
    "name": "Cannon Tower",
    "description": "Torre pesada com alto dano. Lenta mas poderosa.",
    "damage": 50,
    "range": 6.0,
    "attackSpeed": 0.5,
    "cost": 300,
    "spriteUrl": "/assets/sprites/cannon-tower.png",
    "attackSoundUrl": "/assets/sounds/cannon-boom.mp3",
    "towerType": "Physical",
    "damagePerSecond": 25.0
  },
  {
    "name": "Magic Tower",
    "description": "Torre mágica com dano em área. Efetiva contra grupos.",
    "damage": 30,
    "range": 7.0,
    "attackSpeed": 1.0,
    "cost": 250,
    "spriteUrl": "/assets/sprites/magic-tower.png",
    "attackSoundUrl": "/assets/sounds/magic-blast.mp3",
    "towerType": "Magic",
    "damagePerSecond": 30.0
  }
]
```

### 4. Filtrar Torres por Tipo

**GET** `/api/assets/towers/type/{towerType}`

Exemplo: `/api/assets/towers/type/Physical`

---

## 📊 Sistema de Score

### 5. Atualizar Score do Jogador

**POST** `/api/score/update` (Requer autenticação)

**Headers**:
```
Authorization: Bearer {token}
```

**Body**:
```json
{
  "score": 1500
}
```

**Resposta (200 OK)**:
```json
{
  "success": true,
  "highScore": 1500,
  "isNewRecord": true,
  "message": "🎉 Novo recorde pessoal!"
}
```

> **Nota**: O backend só atualiza o `highScore` se o novo score for **maior** que o anterior.

### 6. Buscar Score Atual

**GET** `/api/score/current` (Requer autenticação)

**Resposta**:
```json
{
  "username": "pamela",
  "highScore": 1500
}
```

---

## 🏆 Leaderboard

### 7. Top 10 Jogadores

**GET** `/api/leaderboard/top10`

**Resposta (200 OK)**:
```json
[
  {
    "rank": 1,
    "username": "pamela",
    "highScore": 5000,
    "lastLoginAt": "2026-01-15T16:00:00Z"
  },
  {
    "rank": 2,
    "username": "joao",
    "highScore": 4500,
    "lastLoginAt": "2026-01-15T15:30:00Z"
  }
]
```

### 8. Top N Customizável

**GET** `/api/leaderboard/top/{count}`

Exemplo: `/api/leaderboard/top/50`

---

## 🎮 SignalR (Real-Time / Multiplayer)

### Conectar ao Hub

**URL**: `ws://localhost:5000/hubs/game`

**Exemplo com JavaScript**:
```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl('http://localhost:5000/hubs/game', {
        accessTokenFactory: () => seuTokenJWT
    })
    .build();

await connection.start();
```

### Métodos Disponíveis

#### Enviar Mensagem Broadcast
```javascript
await connection.invoke('BroadcastMessage', 'Olá, todos!');
```

#### Entrar em uma Sala (Partida)
```javascript
await connection.invoke('JoinRoom', 'sala-123');
```

#### Sincronizar Estado do Jogo
```javascript
await connection.invoke('SyncGameState', 'sala-123', {
    wave: 5,
    enemies: [...],
    towers: [...]
});
```

### Eventos que o Frontend Recebe

```javascript
// Mensagem recebida
connection.on('ReceiveMessage', (data) => {
    console.log(data.message);
});

// Usuário entrou na sala
connection.on('UserJoinedRoom', (data) => {
    console.log(`${data.connectionId} entrou`);
});

// Estado do jogo atualizado
connection.on('GameStateUpdated', (data) => {
    // Sincronizar com o jogo local
});
```

---

## 🔧 Exemplo de Integração Completa

```javascript
// 1. Login
const loginResponse = await fetch('http://localhost:5000/api/auth/login', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
        usernameOrEmail: 'pamela',
        password: 'senha123'
    })
});

const { token } = await loginResponse.json();

// 2. Carregar Torres
const towersResponse = await fetch('http://localhost:5000/api/assets/towers');
const towers = await towersResponse.json();

// 3. Jogar o jogo...
// (lógica do Ítalo)

// 4. Game Over - Salvar Score
const scoreResponse = await fetch('http://localhost:5000/api/score/update', {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
    },
    body: JSON.stringify({ score: 1500 })
});

const scoreResult = await scoreResponse.json();
console.log(scoreResult.message); // "🎉 Novo recorde pessoal!"

// 5. Mostrar Leaderboard
const leaderboardResponse = await fetch('http://localhost:5000/api/leaderboard/top10');
const leaderboard = await leaderboardResponse.json();
```

---

## 🐛 Tratamento de Erros

Todos os endpoints retornam erros no formato:

```json
{
  "error": "Mensagem de erro descritiva"
}
```

**Códigos HTTP**:
- `200` - Sucesso
- `400` - Bad Request (validação falhou)
- `401` - Unauthorized (token inválido ou ausente)
- `404` - Not Found
- `500` - Internal Server Error

---

## 📝 Notas Importantes

1. **CORS**: O backend já está configurado para aceitar requests de `localhost:3000` e `localhost:5173`
2. **Token JWT**: Expira em **24 horas**
3. **Senhas**: Hasheadas com BCrypt (nunca armazenadas em texto plano)
4. **Seed Data**: O banco já vem com 3 torres de exemplo (Archer, Cannon, Magic)

---

## 🚀 Próximos Passos para o Frontend

1. **João**: Criar HTML/CSS para telas de login e jogo
2. **Ítalo**: Implementar lógica do jogo e integrar com esta API
3. **Todos**: Testar integração completa

---

**Dúvidas?** Falar com Pamela! 😊
