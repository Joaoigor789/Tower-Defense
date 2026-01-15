# 🎮 Backend.TowerDefense

**Backend API para Tower Defense Game - Clean Architecture com .NET 8**

---

## 👋 Sobre Este Projeto

Olá! Sou **Pamela Menezes**, Lead Backend Engineer deste projeto. Decidi criar este backend seguindo os princípios mais rigorosos de **Clean Architecture** (também conhecida como Onion Architecture) porque acredito que código bem estruturado é código que escala, se mantém e evolui com facilidade.

Este não é apenas mais um backend de jogo. É uma demonstração de como aplicar padrões enterprise-grade em um contexto de game development, provando que **jogos também merecem arquitetura sólida**.

---

## 🏗️ Por Que Clean Architecture?

Quando comecei este projeto, tinha três objetivos principais:

1. **Separação de Responsabilidades**: Cada camada tem um propósito claro e não depende de detalhes de implementação das outras.
2. **Testabilidade**: Posso testar a lógica de negócio sem precisar de um banco de dados ou servidor HTTP.
3. **Flexibilidade**: Posso trocar PostgreSQL por MongoDB, ou EF Core por Dapper, sem alterar uma linha de código no Domain ou Application.

A Clean Architecture me dá tudo isso. É mais trabalho inicial? Sim. Vale a pena? **Absolutamente**.

---

## 📦 Estrutura do Projeto

```
Backend.TowerDefense/
├── TowerDefense.Domain/          # 🏛️ Core - Lógica de Negócio Pura
│   ├── Entities/
│   │   ├── Player.cs             # Entity com Rich Domain Model
│   │   └── TowerMetadata.cs      # Metadados de torres para Assets API
│   └── Interfaces/
│       ├── IPlayerRepository.cs
│       └── ITowerMetadataRepository.cs
│
├── TowerDefense.Application/     # ⚙️ Use Cases e Serviços
│   ├── DTOs/
│   │   ├── Auth/                 # LoginDto, RegisterDto, AuthResponseDto
│   │   └── Assets/               # TowerMetadataDto, LeaderboardEntryDto
│   ├── Services/
│   │   ├── AuthService.cs        # Lógica de Login/Register
│   │   ├── LeaderboardService.cs # Top 10 High Scores
│   │   └── AssetsService.cs      # Metadados de Torres
│   └── Interfaces/
│       └── IJwtTokenGenerator.cs
│
├── TowerDefense.Infrastructure/  # 🔧 Implementações Concretas
│   ├── Data/
│   │   └── ApplicationDbContext.cs  # EF Core + PostgreSQL
│   ├── Repositories/
│   │   ├── PlayerRepository.cs
│   │   └── TowerMetadataRepository.cs
│   └── Auth/
│       └── JwtTokenGenerator.cs     # Geração de tokens JWT
│
└── TowerDefense.API/             # 🌐 Camada de Apresentação
    ├── Controllers/
    │   ├── AuthController.cs        # POST /api/auth/login, /register
    │   ├── AssetsController.cs      # GET /api/assets/towers
    │   └── LeaderboardController.cs # GET /api/leaderboard/top10
    ├── Hubs/
    │   └── GameHub.cs               # SignalR para Real-Time
    └── Program.cs                   # DI Container + Middleware Pipeline
```

---

## 🤔 Decisões Técnicas (E Por Quê)

### Por Que .NET 8?

- **Performance**: .NET 8 é uma das plataformas mais rápidas do mercado (benchmarks provam).
- **Moderno**: C# 12 traz features incríveis (Primary Constructors, Collection Expressions, etc.).
- **Cross-Platform**: Roda em Windows, Linux, macOS (importante para deploy em cloud).
- **Ecossistema**: EF Core, SignalR, JWT... tudo nativo e bem integrado.

### Por Que PostgreSQL?

- **Open-Source**: Sem custos de licença.
- **Robusto**: Usado por empresas como Instagram, Spotify, Reddit.
- **Recursos Avançados**: Suporta JSON, Full-Text Search, e muito mais.
- **Npgsql**: O driver .NET é excelente.

### Por Que JWT ao Invés de Sessions?

- **Stateless**: Não preciso armazenar sessões no servidor (escalabilidade horizontal).
- **Self-Contained**: O token contém todas as informações necessárias.
- **Padrão da Indústria**: Amplamente suportado (mobile, web, desktop).

### Por Que SignalR?

- **Real-Time**: WebSockets nativos para comunicação bidirecional.
- **Escalável**: Suporta milhares de conexões simultâneas.
- **Fácil de Usar**: Abstração de alto nível (não preciso lidar com WebSockets crus).
- **Futuro**: Base para features multiplayer, chat, notificações em tempo real.

### Por Que Assets API ao Invés de Servir Arquivos?

Decidi que o backend retorna **metadados em JSON** (Damage, Range, URLs de sprites/sons) ao invés de servir arquivos binários porque:

- **Separação de Responsabilidades**: Backend fornece dados, CDN/Frontend serve assets.
- **Performance**: JSON é muito mais leve que binários.
- **Escalabilidade**: Posso usar CDN (Cloudflare, AWS CloudFront) para assets estáticos.
- **Flexibilidade**: Posso mudar URLs de assets sem alterar código do jogo.

---

## 🚀 Como Rodar Este Projeto

### Pré-Requisitos

1. **.NET 8 SDK** instalado ([Download aqui](https://dotnet.microsoft.com/download/dotnet/8.0))
2. **PostgreSQL** rodando localmente ou via Docker
3. **PowerShell** (para rodar o script de setup)

### Passo 1: Rodar o Script de Setup

Abra o PowerShell na pasta `Backend.TowerDefense` e execute:

```powershell
.\setup.ps1
```

Este script vai:
- Criar a solution `.sln`
- Criar os 4 projetos `.csproj`
- Adicionar referências entre projetos
- Instalar todos os NuGet packages necessários

### Passo 2: Configurar o Banco de Dados

1. **Criar o banco PostgreSQL**:

```sql
CREATE DATABASE towerdefense;
```

2. **Configurar a connection string** em `TowerDefense.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=towerdefense;Username=SEU_USUARIO;Password=SUA_SENHA"
  }
}
```

3. **Gerar e aplicar migrations**:

```powershell
cd Backend.TowerDefense

# Criar a migration inicial
dotnet ef migrations add InitialCreate --project TowerDefense.Infrastructure --startup-project TowerDefense.API

# Aplicar ao banco
dotnet ef database update --project TowerDefense.Infrastructure --startup-project TowerDefense.API
```

### Passo 3: Configurar JWT Secret Key

**IMPORTANTE**: Mude a `Jwt:SecretKey` no `appsettings.json` para uma chave forte:

```json
{
  "Jwt": {
    "SecretKey": "SUA-CHAVE-SECRETA-FORTE-DE-PELO-MENOS-32-CARACTERES",
    "Issuer": "TowerDefenseAPI",
    "Audience": "TowerDefenseClient",
    "ExpirationHours": "24"
  }
}
```

> ⚠️ **Nunca commite a chave secreta real no Git!** Use variáveis de ambiente em produção.

### Passo 4: Rodar a API

```powershell
cd TowerDefense.API
dotnet run
```

A API vai subir em:
- **HTTP**: `http://localhost:5000`
- **HTTPS**: `https://localhost:5001`
- **Swagger UI**: `http://localhost:5000` (documentação interativa)

---

## 📡 Endpoints Disponíveis

### Autenticação

- **POST** `/api/auth/register` - Registrar novo jogador
- **POST** `/api/auth/login` - Login de jogador existente

### Assets

- **GET** `/api/assets/towers` - Listar todas as torres (metadados)
- **GET** `/api/assets/towers/type/{type}` - Filtrar torres por tipo

### Leaderboard

- **GET** `/api/leaderboard/top10` - Top 10 jogadores
- **GET** `/api/leaderboard/top/{count}` - Top N jogadores (customizável)

### SignalR

- **WebSocket** `/hubs/game` - Hub para comunicação real-time

---

## 🧪 Testando a API

### Usando Swagger UI

1. Acesse `http://localhost:5000`
2. Teste o endpoint `/api/auth/register` para criar um jogador
3. Use o token JWT retornado para autenticar nas próximas requests

### Usando cURL

```bash
# Registrar jogador
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "pamela",
    "email": "pamela@email.com",
    "password": "senha123",
    "confirmPassword": "senha123"
  }'

# Login
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "usernameOrEmail": "pamela",
    "password": "senha123"
  }'

# Listar torres
curl -X GET http://localhost:5000/api/assets/towers
```

---

## 🔐 Segurança

- **Senhas**: Hasheadas com **BCrypt** (salt automático, resistente a brute-force)
- **JWT**: Tokens assinados com **HMAC-SHA256**
- **HTTPS**: Redirecionamento automático em produção
- **CORS**: Configurado para permitir apenas origens confiáveis

---

## 🎯 Próximos Passos

- [ ] Implementar endpoint para atualizar HighScore
- [ ] Adicionar sistema de achievements
- [ ] Implementar matchmaking para multiplayer
- [ ] Adicionar rate limiting (proteção contra DDoS)
- [ ] Implementar refresh tokens (JWT de longa duração)
- [ ] Adicionar testes unitários e de integração
- [ ] Deploy em Azure/AWS

---

## 📚 Tecnologias Utilizadas

- **C# 12** - Linguagem
- **.NET 8** - Framework
- **ASP.NET Core** - Web API
- **Entity Framework Core** - ORM
- **PostgreSQL** - Banco de Dados
- **Npgsql** - Driver PostgreSQL para .NET
- **SignalR** - Real-Time Communication
- **JWT** - Autenticação
- **BCrypt.Net** - Hash de Senhas
- **Swagger/OpenAPI** - Documentação da API

---

## 👩‍💻 Autora

**Pamela Menezes**  
Lead Backend Engineer  
[GitHub](https://github.com/Joaoigor789/Tower-Defense)

---

## 📄 Licença

Este projeto é open-source e está disponível sob a licença MIT.

---

**Feito com 💜 e muita ☕ por Pamela Menezes**
