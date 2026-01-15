# ============================================================================
# Stack de Elite de Testes - Installation Script
# ============================================================================
# Instala apenas os pacotes "Best-in-Class" para testes enterprise-grade
# ============================================================================

Write-Host "🧪 Instalando Stack de Elite de Testes..." -ForegroundColor Cyan

# Navegar para o projeto de testes
cd TowerDefense.Tests

Write-Host ""
Write-Host "📚 Instalando pacotes NuGet..." -ForegroundColor Yellow

# NSubstitute - Mocking (melhor sintaxe que Moq)
Write-Host "  → NSubstitute (Mocking)" -ForegroundColor Gray
dotnet add package NSubstitute

# FluentAssertions - Já instalado, mas garantir
Write-Host "  → FluentAssertions (Assertions)" -ForegroundColor Gray
dotnet add package FluentAssertions

# Microsoft.AspNetCore.Mvc.Testing - Já instalado
Write-Host "  → Microsoft.AspNetCore.Mvc.Testing (Integration Tests)" -ForegroundColor Gray
dotnet add package Microsoft.AspNetCore.Mvc.Testing

# Bogus - Já instalado
Write-Host "  → Bogus (Fake Data)" -ForegroundColor Gray
dotnet add package Bogus

# EF Core InMemory - Para testes de integração
Write-Host "  → Microsoft.EntityFrameworkCore.InMemory (In-Memory DB)" -ForegroundColor Gray
dotnet add package Microsoft.EntityFrameworkCore.InMemory

Write-Host ""
Write-Host "🔧 Instalando Stryker.NET (Mutation Testing)..." -ForegroundColor Yellow
dotnet tool install -g dotnet-stryker

Write-Host ""
Write-Host "✅ Stack de Elite instalada com sucesso!" -ForegroundColor Green
Write-Host ""
Write-Host "📦 Pacotes instalados:" -ForegroundColor Cyan
Write-Host "   ✓ xUnit (framework)" -ForegroundColor Gray
Write-Host "   ✓ NSubstitute (mocking - melhor que Moq)" -ForegroundColor Gray
Write-Host "   ✓ FluentAssertions (assertions legíveis)" -ForegroundColor Gray
Write-Host "   ✓ WebApplicationFactory (integration tests)" -ForegroundColor Gray
Write-Host "   ✓ Bogus (fake data)" -ForegroundColor Gray
Write-Host "   ✓ EF Core InMemory (in-memory database)" -ForegroundColor Gray
Write-Host "   ✓ Stryker.NET (mutation testing)" -ForegroundColor Gray
Write-Host ""
Write-Host "🚀 Próximos passos:" -ForegroundColor Cyan
Write-Host "   1. Rodar testes: dotnet test" -ForegroundColor Gray
Write-Host "   2. Mutation testing: dotnet stryker" -ForegroundColor Gray
Write-Host ""
