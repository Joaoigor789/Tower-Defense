namespace TowerDefense.Application.Interfaces;

/// <summary>
/// Interface para geração de tokens JWT.
/// 
/// Por que interface?
/// - Testabilidade: Posso mockar a geração de tokens nos testes.
/// - Inversão de Dependência: Application não depende de implementação concreta.
/// - Flexibilidade: Posso trocar JWT por outro mecanismo (OAuth, etc.) no futuro.
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// Gera um token JWT para um jogador autenticado.
    /// </summary>
    /// <param name="playerId">ID do jogador</param>
    /// <param name="username">Username do jogador</param>
    /// <param name="email">Email do jogador</param>
    /// <returns>Token JWT assinado</returns>
    string GenerateToken(Guid playerId, string username, string email);

    /// <summary>
    /// Valida um token JWT e retorna o ID do jogador se válido.
    /// Retorna null se o token for inválido ou expirado.
    /// </summary>
    Guid? ValidateToken(string token);
}
