using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TowerDefense.Application.Interfaces;

namespace TowerDefense.Infrastructure.Auth;

/// <summary>
/// Implementação concreta do gerador de tokens JWT.
/// 
/// Por que JWT?
/// - Stateless: Não preciso armazenar sessões no servidor (escalabilidade).
/// - Self-contained: O token contém todas as informações necessárias.
/// - Padrão da indústria: Amplamente suportado e seguro.
/// </summary>
public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gera um token JWT assinado para um jogador.
    /// </summary>
    public string GenerateToken(Guid playerId, string username, string email)
    {
        // 1. Buscar configurações do appsettings.json
        var secretKey = _configuration["Jwt:SecretKey"] 
            ?? throw new InvalidOperationException("Jwt:SecretKey não configurado");
        
        var issuer = _configuration["Jwt:Issuer"] ?? "TowerDefenseAPI";
        var audience = _configuration["Jwt:Audience"] ?? "TowerDefenseClient";
        var expirationHours = int.Parse(_configuration["Jwt:ExpirationHours"] ?? "24");

        // 2. Criar as Claims (informações que vão dentro do token)
        // Por que Claims? Porque são a forma padrão de armazenar dados no JWT.
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, playerId.ToString()), // Subject (ID do usuário)
            new Claim(JwtRegisteredClaimNames.UniqueName, username),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT ID (único por token)
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()) // Issued At
        };

        // 3. Criar a chave de assinatura
        // Por que HMAC-SHA256? Porque é rápido e seguro para tokens JWT.
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // 4. Criar o token
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(expirationHours),
            signingCredentials: credentials
        );

        // 5. Serializar o token para string
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Valida um token JWT e retorna o ID do jogador se válido.
    /// </summary>
    public Guid? ValidateToken(string token)
    {
        try
        {
            var secretKey = _configuration["Jwt:SecretKey"] 
                ?? throw new InvalidOperationException("Jwt:SecretKey não configurado");
            
            var issuer = _configuration["Jwt:Issuer"] ?? "TowerDefenseAPI";
            var audience = _configuration["Jwt:Audience"] ?? "TowerDefenseClient";

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);

            // Parâmetros de validação
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true, // Valida se o token não expirou
                ClockSkew = TimeSpan.Zero // Sem tolerância de tempo
            };

            // Validar o token
            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);

            // Extrair o ID do jogador do claim "sub"
            var playerIdClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            
            if (playerIdClaim != null && Guid.TryParse(playerIdClaim, out var playerId))
            {
                return playerId;
            }

            return null;
        }
        catch
        {
            // Token inválido, expirado, ou malformado
            return null;
        }
    }
}
