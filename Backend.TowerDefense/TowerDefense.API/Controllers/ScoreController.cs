using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TowerDefense.Domain.Interfaces;

namespace TowerDefense.API.Controllers;

/// <summary>
/// Controller responsável por atualizar scores dos jogadores.
/// Requer autenticação JWT.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize] // Requer JWT token válido
public class ScoreController : ControllerBase
{
    private readonly IPlayerRepository _playerRepository;
    private readonly ILogger<ScoreController> _logger;

    public ScoreController(
        IPlayerRepository playerRepository,
        ILogger<ScoreController> logger)
    {
        _playerRepository = playerRepository;
        _logger = logger;
    }

    /// <summary>
    /// Atualiza o HighScore do jogador autenticado.
    /// POST /api/score/update
    /// </summary>
    [HttpPost("update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateScore([FromBody] UpdateScoreRequest request)
    {
        try
        {
            // Extrair ID do jogador do JWT token
            var playerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(playerIdClaim) || !Guid.TryParse(playerIdClaim, out var playerId))
            {
                return Unauthorized(new { error = "Token inválido" });
            }

            _logger.LogInformation("Atualizando score do jogador {PlayerId}: {Score}", playerId, request.Score);

            // Buscar jogador
            var player = await _playerRepository.GetByIdAsync(playerId);
            
            if (player == null)
            {
                return NotFound(new { error = "Jogador não encontrado" });
            }

            // Atualizar HighScore (o método só atualiza se for maior)
            var oldScore = player.HighScore;
            player.UpdateHighScore(request.Score);
            
            // Salvar no banco
            await _playerRepository.UpdateAsync(player);
            await _playerRepository.SaveChangesAsync();

            var isNewRecord = player.HighScore > oldScore;

            _logger.LogInformation(
                "Score atualizado para jogador {Username}: {OldScore} -> {NewScore} (Novo recorde: {IsNewRecord})",
                player.Username,
                oldScore,
                player.HighScore,
                isNewRecord
            );

            return Ok(new
            {
                success = true,
                highScore = player.HighScore,
                isNewRecord,
                message = isNewRecord ? "🎉 Novo recorde pessoal!" : "Score registrado"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar score");
            return StatusCode(500, new { error = "Erro ao atualizar score" });
        }
    }

    /// <summary>
    /// Retorna o HighScore atual do jogador autenticado.
    /// GET /api/score/current
    /// </summary>
    [HttpGet("current")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCurrentScore()
    {
        try
        {
            var playerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(playerIdClaim) || !Guid.TryParse(playerIdClaim, out var playerId))
            {
                return Unauthorized(new { error = "Token inválido" });
            }

            var player = await _playerRepository.GetByIdAsync(playerId);
            
            if (player == null)
            {
                return NotFound(new { error = "Jogador não encontrado" });
            }

            return Ok(new
            {
                username = player.Username,
                highScore = player.HighScore
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar score");
            return StatusCode(500, new { error = "Erro ao buscar score" });
        }
    }
}

/// <summary>
/// Request para atualizar score
/// </summary>
public class UpdateScoreRequest
{
    public int Score { get; set; }
}
