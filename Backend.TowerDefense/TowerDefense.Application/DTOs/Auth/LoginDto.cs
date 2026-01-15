using System.ComponentModel.DataAnnotations;

namespace TowerDefense.Application.DTOs.Auth;

/// <summary>
/// DTO para request de login.
/// 
/// Por que DTO separado da Entity?
/// - Separação de responsabilidades: DTOs são para transporte de dados (API),
///   Entities são para lógica de negócio (Domain).
/// - Segurança: Não exponho a estrutura interna do Domain para o cliente.
/// - Validação: Posso usar Data Annotations para validar input da API.
/// </summary>
public class LoginDto
{
    /// <summary>
    /// Username ou Email do jogador.
    /// Aceito ambos para melhor UX (jogador pode usar qualquer um).
    /// </summary>
    [Required(ErrorMessage = "Username ou Email é obrigatório")]
    public string UsernameOrEmail { get; set; } = string.Empty;

    /// <summary>
    /// Senha em texto plano (será hasheada no backend).
    /// NUNCA armazenamos senhas em texto plano no banco.
    /// </summary>
    [Required(ErrorMessage = "Senha é obrigatória")]
    [MinLength(6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres")]
    public string Password { get; set; } = string.Empty;
}
