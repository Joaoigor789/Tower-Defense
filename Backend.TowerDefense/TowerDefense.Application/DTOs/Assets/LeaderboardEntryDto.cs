namespace TowerDefense.Application.DTOs.Assets;

/// <summary>
/// DTO para retornar informações de um jogador no Leaderboard.
/// Não exponho dados sensíveis como Email ou PasswordHash.
/// </summary>
public class LeaderboardEntryDto
{
    /// <summary>
    /// Posição no ranking (1 = primeiro lugar).
    /// </summary>
    public int Rank { get; set; }

    /// <summary>
    /// Username do jogador.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// HighScore do jogador.
    /// </summary>
    public int HighScore { get; set; }

    /// <summary>
    /// Data do último login (opcional, para mostrar atividade).
    /// </summary>
    public DateTime? LastLoginAt { get; set; }
}
