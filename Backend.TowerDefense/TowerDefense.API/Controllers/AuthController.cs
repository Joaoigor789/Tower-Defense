using Microsoft.AspNetCore.Mvc;
using TowerDefense.Application.DTOs.Auth;
using TowerDefense.Application.Services;

namespace TowerDefense.API.Controllers;

/// <summary>
/// Controller responsável por autenticação (Login e Register).
/// 
/// Por que [ApiController]?
/// - Habilita validação automática de ModelState (Data Annotations).
/// - Inferência automática de [FromBody], [FromQuery], etc.
/// - Respostas automáticas de erro 400 para validações.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        AuthService authService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Endpoint para registro de novo jogador.
    /// POST /api/auth/register
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        try
        {
            _logger.LogInformation("Tentativa de registro para username: {Username}", dto.Username);

            var response = await _authService.RegisterAsync(dto);

            _logger.LogInformation("Registro bem-sucedido para username: {Username}", dto.Username);

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            // Username ou Email já existe
            _logger.LogWarning("Falha no registro: {Message}", ex.Message);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            // Erro inesperado
            _logger.LogError(ex, "Erro inesperado no registro");
            return StatusCode(500, new { error = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Endpoint para login de jogador existente.
    /// POST /api/auth/login
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            _logger.LogInformation("Tentativa de login para: {UsernameOrEmail}", dto.UsernameOrEmail);

            var response = await _authService.LoginAsync(dto);

            _logger.LogInformation("Login bem-sucedido para: {Username}", response.Username);

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            // Credenciais inválidas
            _logger.LogWarning("Falha no login: {Message}", ex.Message);
            return Unauthorized(new { error = "Credenciais inválidas" });
        }
        catch (Exception ex)
        {
            // Erro inesperado
            _logger.LogError(ex, "Erro inesperado no login");
            return StatusCode(500, new { error = "Erro interno do servidor" });
        }
    }
}
