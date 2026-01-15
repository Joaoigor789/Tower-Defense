using TowerDefense.Application.DTOs.Auth;
using TowerDefense.Application.Interfaces;
using TowerDefense.Domain.Entities;
using TowerDefense.Domain.Interfaces;

namespace TowerDefense.Application.Services;

/// <summary>
/// Service responsável pela lógica de autenticação (Login e Register).
/// 
/// Por que Service ao invés de colocar tudo no Controller?
/// - Single Responsibility: Controller cuida de HTTP, Service cuida de lógica de negócio.
/// - Testabilidade: Posso testar a lógica sem precisar de um servidor HTTP.
/// - Reusabilidade: Posso usar este Service em outros contextos (CLI, Jobs, etc.).
/// </summary>
public class AuthService
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(
        IPlayerRepository playerRepository,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _playerRepository = playerRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    /// <summary>
    /// Registra um novo jogador no sistema.
    /// </summary>
    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        // 1. Validar se username já existe
        var existingByUsername = await _playerRepository.GetByUsernameAsync(dto.Username);
        if (existingByUsername != null)
        {
            throw new InvalidOperationException("Username já está em uso");
        }

        // 2. Validar se email já existe
        var existingByEmail = await _playerRepository.GetByEmailAsync(dto.Email);
        if (existingByEmail != null)
        {
            throw new InvalidOperationException("Email já está em uso");
        }

        // 3. Hashear a senha usando BCrypt
        // Por que BCrypt? Porque é o padrão da indústria para hash de senhas.
        // Ele adiciona salt automaticamente e é resistente a brute-force.
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        // 4. Criar a entity Player usando o Factory Method
        var player = Player.Create(dto.Username, dto.Email, passwordHash);

        // 5. Salvar no banco
        await _playerRepository.AddAsync(player);
        await _playerRepository.SaveChangesAsync();

        // 6. Gerar token JWT
        var token = _jwtTokenGenerator.GenerateToken(player.Id, player.Username, player.Email);

        // 7. Retornar resposta
        return new AuthResponseDto
        {
            Token = token,
            Username = player.Username,
            Email = player.Email,
            HighScore = player.HighScore,
            ExpiresAt = DateTime.UtcNow.AddHours(24) // Token válido por 24h
        };
    }

    /// <summary>
    /// Autentica um jogador existente.
    /// </summary>
    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        // 1. Buscar jogador por username ou email
        // Por que aceitar ambos? Melhor UX - jogador pode usar qualquer um.
        Player? player = await _playerRepository.GetByUsernameAsync(dto.UsernameOrEmail);
        
        if (player == null)
        {
            // Tentar buscar por email se não encontrou por username
            player = await _playerRepository.GetByEmailAsync(dto.UsernameOrEmail);
        }

        // 2. Validar se jogador existe
        if (player == null)
        {
            throw new UnauthorizedAccessException("Credenciais inválidas");
        }

        // 3. Verificar senha usando BCrypt
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, player.PasswordHash);
        
        if (!isPasswordValid)
        {
            throw new UnauthorizedAccessException("Credenciais inválidas");
        }

        // 4. Registrar o login
        player.RecordLogin();
        await _playerRepository.UpdateAsync(player);
        await _playerRepository.SaveChangesAsync();

        // 5. Gerar token JWT
        var token = _jwtTokenGenerator.GenerateToken(player.Id, player.Username, player.Email);

        // 6. Retornar resposta
        return new AuthResponseDto
        {
            Token = token,
            Username = player.Username,
            Email = player.Email,
            HighScore = player.HighScore,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };
    }
}
