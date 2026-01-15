using TowerDefense.Application.DTOs.Assets;
using TowerDefense.Domain.Interfaces;

namespace TowerDefense.Application.Services;

/// <summary>
/// Service responsável pela lógica do Leaderboard.
/// </summary>
public class LeaderboardService
{
    private readonly IPlayerRepository _playerRepository;

    public LeaderboardService(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    /// <summary>
    /// Retorna os Top N jogadores ordenados por HighScore.
    /// </summary>
    public async Task<IEnumerable<LeaderboardEntryDto>> GetTopPlayersAsync(int count = 10)
    {
        // 1. Buscar top players do repository
        var topPlayers = await _playerRepository.GetTopPlayersByScoreAsync(count);

        // 2. Transformar em DTOs e adicionar o Rank
        var leaderboard = topPlayers
            .Select((player, index) => new LeaderboardEntryDto
            {
                Rank = index + 1, // Rank começa em 1
                Username = player.Username,
                HighScore = player.HighScore,
                LastLoginAt = player.LastLoginAt
            })
            .ToList();

        return leaderboard;
    }
}
