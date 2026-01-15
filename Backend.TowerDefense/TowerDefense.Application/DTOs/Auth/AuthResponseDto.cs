namespace TowerDefense.Application.DTOs.Auth;

/// <summary>
/// DTO de resposta para autenticação bem-sucedida.
/// Retorna o token JWT e informações básicas do jogador.
/// </summary>
public class AuthResponseDto
{
    /// <summary>
    /// Token JWT para autenticação nas próximas requests.
    /// O frontend deve armazenar isso (localStorage ou cookie httpOnly).
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Username do jogador autenticado.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Email do jogador autenticado.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// HighScore atual do jogador.
    /// Útil para exibir na UI logo após login.
    /// </summary>
    public int HighScore { get; set; }

    /// <summary>
    /// Data de expiração do token.
    /// O frontend pode usar isso para fazer refresh automático.
    /// </summary>
    public DateTime ExpiresAt { get; set; }
}
