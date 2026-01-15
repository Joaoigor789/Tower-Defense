using FluentAssertions;
using NSubstitute;
using TowerDefense.Application.DTOs.Auth;
using TowerDefense.Application.Interfaces;
using TowerDefense.Application.Services;
using TowerDefense.Domain.Entities;
using TowerDefense.Domain.Interfaces;

namespace TowerDefense.Tests.UnitTests;

/// <summary>
/// Testes unitários para AuthService usando NSubstitute para mocking.
/// Demonstra a sintaxe limpa do NSubstitute vs Moq.
/// </summary>
public class AuthServiceTests
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        // NSubstitute: Sintaxe limpa para criar mocks
        _playerRepository = Substitute.For<IPlayerRepository>();
        _jwtTokenGenerator = Substitute.For<IJwtTokenGenerator>();
        
        _authService = new AuthService(_playerRepository, _jwtTokenGenerator);
    }

    [Fact]
    public async Task RegisterAsync_WithValidData_ShouldCreatePlayerAndReturnToken()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "pamela",
            Email = "pamela@test.com",
            Password = "senha123",
            ConfirmPassword = "senha123"
        };

        var expectedToken = "fake-jwt-token";
        var expectedExpiration = DateTime.UtcNow.AddHours(24);

        // NSubstitute: Configurar retorno do mock
        _playerRepository.GetByUsernameAsync(registerDto.Username)
            .Returns(Task.FromResult<Player?>(null));
        
        _playerRepository.GetByEmailAsync(registerDto.Email)
            .Returns(Task.FromResult<Player?>(null));

        _jwtTokenGenerator.GenerateToken(Arg.Any<Guid>(), registerDto.Username, registerDto.Email)
            .Returns((expectedToken, expectedExpiration));

        // Act
        var result = await _authService.RegisterAsync(registerDto);

        // Assert - FluentAssertions: Leitura natural
        result.Should().NotBeNull();
        result.Token.Should().Be(expectedToken);
        result.Username.Should().Be(registerDto.Username);
        result.Email.Should().Be(registerDto.Email);
        result.HighScore.Should().Be(0);
        result.ExpiresAt.Should().BeCloseTo(expectedExpiration, TimeSpan.FromSeconds(1));

        // Verificar que o repositório foi chamado corretamente
        await _playerRepository.Received(1).AddAsync(Arg.Any<Player>());
        await _playerRepository.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task RegisterAsync_WithExistingUsername_ShouldThrowException()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "pamela",
            Email = "pamela@test.com",
            Password = "senha123",
            ConfirmPassword = "senha123"
        };

        var existingPlayer = Player.Create("pamela", "other@email.com", "hashedpassword");

        _playerRepository.GetByUsernameAsync(registerDto.Username)
            .Returns(Task.FromResult<Player?>(existingPlayer));

        // Act
        Func<Task> act = async () => await _authService.RegisterAsync(registerDto);

        // Assert - FluentAssertions: Sintaxe expressiva para exceções
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*já está em uso*");
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            UsernameOrEmail = "pamela",
            Password = "senha123"
        };

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(loginDto.Password);
        var player = Player.Create("pamela", "pamela@test.com", hashedPassword);

        _playerRepository.GetByUsernameAsync(loginDto.UsernameOrEmail)
            .Returns(Task.FromResult<Player?>(player));

        var expectedToken = "fake-jwt-token";
        var expectedExpiration = DateTime.UtcNow.AddHours(24);

        _jwtTokenGenerator.GenerateToken(player.Id, player.Username, player.Email)
            .Returns((expectedToken, expectedExpiration));

        // Act
        var result = await _authService.LoginAsync(loginDto);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be(expectedToken);
        result.Username.Should().Be(player.Username);
        result.Email.Should().Be(player.Email);

        // Verificar que RecordLogin foi chamado
        await _playerRepository.Received(1).UpdateAsync(Arg.Is<Player>(p => p.Id == player.Id));
        await _playerRepository.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ShouldThrowException()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            UsernameOrEmail = "pamela",
            Password = "senhaerrada"
        };

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("senha123");
        var player = Player.Create("pamela", "pamela@test.com", hashedPassword);

        _playerRepository.GetByUsernameAsync(loginDto.UsernameOrEmail)
            .Returns(Task.FromResult<Player?>(player));

        // Act
        Func<Task> act = async () => await _authService.LoginAsync(loginDto);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*inválidas*");
    }
}
