# ============================================================================
# TowerDefense.Tests - Setup Script
# ============================================================================
# Este script cria o projeto de testes e instala todas as ferramentas de QA
# ============================================================================

Write-Host "🧪 Criando projeto de testes..." -ForegroundColor Cyan

# Criar projeto xUnit
dotnet new xunit -n TowerDefense.Tests -f net8.0

# Adicionar à solution
dotnet sln add TowerDefense.Tests/TowerDefense.Tests.csproj

# Adicionar referências aos projetos
dotnet add TowerDefense.Tests/TowerDefense.Tests.csproj reference TowerDefense.Domain/TowerDefense.Domain.csproj
dotnet add TowerDefense.Tests/TowerDefense.Tests.csproj reference TowerDefense.Application/TowerDefense.Application.csproj
dotnet add TowerDefense.Tests/TowerDefense.Tests.csproj reference TowerDefense.Infrastructure/TowerDefense.Infrastructure.csproj
dotnet add TowerDefense.Tests/TowerDefense.Tests.csproj reference TowerDefense.API/TowerDefense.API.csproj

# ============================================================================
# INSTALAR PACOTES DE TESTE
# ============================================================================

Write-Host ""
Write-Host "📚 Instalando pacotes de teste..." -ForegroundColor Yellow

# FluentAssertions - Assertions legíveis
dotnet add TowerDefense.Tests/TowerDefense.Tests.csproj package FluentAssertions

# Bogus - Geração de dados fake
dotnet add TowerDefense.Tests/TowerDefense.Tests.csproj package Bogus

# NSubstitute - Mocking
dotnet add TowerDefense.Tests/TowerDefense.Tests.csproj package NSubstitute

# WebApplicationFactory - Testes de integração
dotnet add TowerDefense.Tests/TowerDefense.Tests.csproj package Microsoft.AspNetCore.Mvc.Testing

# NetArchTest - Testes de arquitetura
dotnet add TowerDefense.Tests/TowerDefense.Tests.csproj package NetArchTest.Rules

# EF Core InMemory - Banco em memória para testes
dotnet add TowerDefense.Tests/TowerDefense.Tests.csproj package Microsoft.EntityFrameworkCore.InMemory

# ============================================================================
# CRIAR ESTRUTURA DE PASTAS
# ============================================================================

Write-Host ""
Write-Host "📁 Criando estrutura de pastas..." -ForegroundColor Yellow

New-Item -ItemType Directory -Force -Path "TowerDefense.Tests/UnitTests"
New-Item -ItemType Directory -Force -Path "TowerDefense.Tests/IntegrationTests"
New-Item -ItemType Directory -Force -Path "TowerDefense.Tests/ArchitectureTests"

# Remover UnitTest1.cs padrão
Remove-Item "TowerDefense.Tests/UnitTest1.cs" -ErrorAction SilentlyContinue

Write-Host ""
Write-Host "✅ Projeto de testes criado com sucesso!" -ForegroundColor Green
Write-Host ""
Write-Host "📦 Pacotes instalados:" -ForegroundColor Cyan
Write-Host "   ✓ xUnit (framework de testes)" -ForegroundColor Gray
Write-Host "   ✓ FluentAssertions (assertions legíveis)" -ForegroundColor Gray
Write-Host "   ✓ Bogus (geração de dados fake)" -ForegroundColor Gray
Write-Host "   ✓ NSubstitute (mocking)" -ForegroundColor Gray
Write-Host "   ✓ WebApplicationFactory (testes de integração)" -ForegroundColor Gray
Write-Host "   ✓ NetArchTest (testes de arquitetura)" -ForegroundColor Gray
Write-Host ""
