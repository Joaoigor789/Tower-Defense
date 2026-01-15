# ============================================================================
# Backend.TowerDefense - Setup Script
# ============================================================================
# Este script cria a estrutura completa da solution seguindo Clean Architecture
# Autor: Pamela Menezes
# Stack: .NET 8, EF Core, PostgreSQL, SignalR, JWT
# ============================================================================

Write-Host "🎮 Iniciando setup do Backend.TowerDefense..." -ForegroundColor Cyan
Write-Host ""

# Criar a Solution
Write-Host "📦 Criando Solution..." -ForegroundColor Yellow
dotnet new sln -n TowerDefense

# ============================================================================
# PARTE 1: DOMAIN LAYER (Core - Sem dependências)
# ============================================================================
Write-Host ""
Write-Host "🏛️  Criando Domain Layer..." -ForegroundColor Yellow
dotnet new classlib -n TowerDefense.Domain -f net8.0
dotnet sln add TowerDefense.Domain/TowerDefense.Domain.csproj

# Criar estrutura de pastas do Domain
New-Item -ItemType Directory -Force -Path "TowerDefense.Domain/Entities"
New-Item -ItemType Directory -Force -Path "TowerDefense.Domain/Interfaces"

# Remover Class1.cs padrão
Remove-Item "TowerDefense.Domain/Class1.cs" -ErrorAction SilentlyContinue

# ============================================================================
# PARTE 2: APPLICATION LAYER (Use Cases, DTOs, Services)
# ============================================================================
Write-Host ""
Write-Host "⚙️  Criando Application Layer..." -ForegroundColor Yellow
dotnet new classlib -n TowerDefense.Application -f net8.0
dotnet sln add TowerDefense.Application/TowerDefense.Application.csproj

# Adicionar referência ao Domain
dotnet add TowerDefense.Application/TowerDefense.Application.csproj reference TowerDefense.Domain/TowerDefense.Domain.csproj

# Criar estrutura de pastas do Application
New-Item -ItemType Directory -Force -Path "TowerDefense.Application/DTOs/Auth"
New-Item -ItemType Directory -Force -Path "TowerDefense.Application/DTOs/Assets"
New-Item -ItemType Directory -Force -Path "TowerDefense.Application/Services"
New-Item -ItemType Directory -Force -Path "TowerDefense.Application/Interfaces"

# Remover Class1.cs padrão
Remove-Item "TowerDefense.Application/Class1.cs" -ErrorAction SilentlyContinue

# ============================================================================
# PARTE 3: INFRASTRUCTURE LAYER (EF Core, Repositories, JWT)
# ============================================================================
Write-Host ""
Write-Host "🔧 Criando Infrastructure Layer..." -ForegroundColor Yellow
dotnet new classlib -n TowerDefense.Infrastructure -f net8.0
dotnet sln add TowerDefense.Infrastructure/TowerDefense.Infrastructure.csproj

# Adicionar referências
dotnet add TowerDefense.Infrastructure/TowerDefense.Infrastructure.csproj reference TowerDefense.Domain/TowerDefense.Domain.csproj
dotnet add TowerDefense.Infrastructure/TowerDefense.Infrastructure.csproj reference TowerDefense.Application/TowerDefense.Application.csproj

# Adicionar NuGet packages para Infrastructure
Write-Host "📚 Instalando pacotes NuGet para Infrastructure..." -ForegroundColor Magenta
dotnet add TowerDefense.Infrastructure/TowerDefense.Infrastructure.csproj package Microsoft.EntityFrameworkCore
dotnet add TowerDefense.Infrastructure/TowerDefense.Infrastructure.csproj package Microsoft.EntityFrameworkCore.Design
dotnet add TowerDefense.Infrastructure/TowerDefense.Infrastructure.csproj package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add TowerDefense.Infrastructure/TowerDefense.Infrastructure.csproj package Microsoft.IdentityModel.Tokens
dotnet add TowerDefense.Infrastructure/TowerDefense.Infrastructure.csproj package System.IdentityModel.Tokens.Jwt

# Adicionar BCrypt.Net para hash de senhas no Application
Write-Host "📚 Instalando BCrypt.Net para Application..." -ForegroundColor Magenta
dotnet add TowerDefense.Application/TowerDefense.Application.csproj package BCrypt.Net-Next

# Criar estrutura de pastas do Infrastructure
New-Item -ItemType Directory -Force -Path "TowerDefense.Infrastructure/Data"
New-Item -ItemType Directory -Force -Path "TowerDefense.Infrastructure/Repositories"
New-Item -ItemType Directory -Force -Path "TowerDefense.Infrastructure/Auth"

# Remover Class1.cs padrão
Remove-Item "TowerDefense.Infrastructure/Class1.cs" -ErrorAction SilentlyContinue

# ============================================================================
# PARTE 4: API LAYER (Controllers, SignalR Hubs, DI)
# ============================================================================
Write-Host ""
Write-Host "🌐 Criando API Layer..." -ForegroundColor Yellow
dotnet new webapi -n TowerDefense.API -f net8.0 --use-controllers
dotnet sln add TowerDefense.API/TowerDefense.API.csproj

# Adicionar referências
dotnet add TowerDefense.API/TowerDefense.API.csproj reference TowerDefense.Application/TowerDefense.Application.csproj
dotnet add TowerDefense.API/TowerDefense.API.csproj reference TowerDefense.Infrastructure/TowerDefense.Infrastructure.csproj

# Adicionar NuGet packages para API
Write-Host "📚 Instalando pacotes NuGet para API..." -ForegroundColor Magenta
dotnet add TowerDefense.API/TowerDefense.API.csproj package Microsoft.AspNetCore.SignalR
dotnet add TowerDefense.API/TowerDefense.API.csproj package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add TowerDefense.API/TowerDefense.API.csproj package Microsoft.EntityFrameworkCore.Tools

# Criar estrutura de pastas do API
New-Item -ItemType Directory -Force -Path "TowerDefense.API/Hubs"

# Remover arquivos de exemplo
Remove-Item "TowerDefense.API/Controllers/WeatherForecastController.cs" -ErrorAction SilentlyContinue
Remove-Item "TowerDefense.API/WeatherForecast.cs" -ErrorAction SilentlyContinue

# ============================================================================
# FINALIZAÇÃO
# ============================================================================
Write-Host ""
Write-Host "✅ Setup concluído com sucesso!" -ForegroundColor Green
Write-Host ""
Write-Host "📁 Estrutura criada:" -ForegroundColor Cyan
Write-Host "   ├── TowerDefense.Domain (Core - Entities, Interfaces)" -ForegroundColor Gray
Write-Host "   ├── TowerDefense.Application (Use Cases, DTOs, Services)" -ForegroundColor Gray
Write-Host "   ├── TowerDefense.Infrastructure (EF Core, Repositories, JWT)" -ForegroundColor Gray
Write-Host "   └── TowerDefense.API (Controllers, SignalR, DI)" -ForegroundColor Gray
Write-Host ""
Write-Host "🚀 Próximos passos:" -ForegroundColor Yellow
Write-Host "   1. Adicionar os arquivos .cs fornecidos pela IA" -ForegroundColor White
Write-Host "   2. Configurar connection string no appsettings.json" -ForegroundColor White
Write-Host "   3. Rodar: dotnet ef migrations add InitialCreate --project TowerDefense.Infrastructure --startup-project TowerDefense.API" -ForegroundColor White
Write-Host "   4. Rodar: dotnet ef database update --project TowerDefense.Infrastructure --startup-project TowerDefense.API" -ForegroundColor White
Write-Host "   5. Executar: dotnet run --project TowerDefense.API" -ForegroundColor White
Write-Host ""
