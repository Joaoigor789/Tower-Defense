using Microsoft.AspNetCore.Mvc;
using TowerDefense.Application.DTOs.Assets;
using TowerDefense.Application.Services;

namespace TowerDefense.API.Controllers;

/// <summary>
/// Controller responsável pelo Leaderboard (ranking de jogadores).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LeaderboardController : ControllerBase
{
    private readonly LeaderboardService _leaderboardService;
    private readonly ILogger<LeaderboardController> _logger;

    public LeaderboardController(
        LeaderboardService leaderboardService,
        ILogger<LeaderboardController> logger)
    {
        _leaderboardService = leaderboardService;
        _logger = logger;
    }

    /// <summary>
    /// Retorna os Top 10 jogadores com maior HighScore.
    /// GET /api/leaderboard/top10
    /// 
    /// O frontend usa isso para exibir o ranking global.
    /// </summary>
    [HttpGet("top10")]
    [ProducesResponseType(typeof(IEnumerable<LeaderboardEntryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTop10()
    {
        try
        {
            _logger.LogInformation("Requisição para Top 10 do Leaderboard");

            var leaderboard = await _leaderboardService.GetTopPlayersAsync(10);

            return Ok(leaderboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar leaderboard");
            return StatusCode(500, new { error = "Erro ao buscar leaderboard" });
        }
    }

    /// <summary>
    /// Retorna os Top N jogadores (customizável).
    /// GET /api/leaderboard/top/{count}
    /// 
    /// Exemplo: GET /api/leaderboard/top/50
    /// </summary>
    [HttpGet("top/{count:int}")]
    [ProducesResponseType(typeof(IEnumerable<LeaderboardEntryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTopN(int count)
    {
        try
        {
            // Validação: limitar a 100 para evitar queries muito grandes
            if (count < 1 || count > 100)
            {
                return BadRequest(new { error = "Count deve estar entre 1 e 100" });
            }

            _logger.LogInformation("Requisição para Top {Count} do Leaderboard", count);

            var leaderboard = await _leaderboardService.GetTopPlayersAsync(count);

            return Ok(leaderboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar leaderboard");
            return StatusCode(500, new { error = "Erro ao buscar leaderboard" });
        }
    }
}
