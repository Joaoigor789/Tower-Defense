using System.ComponentModel.DataAnnotations;

namespace TowerDefense.Application.DTOs.Auth;

/// <summary>
/// DTO para request de registro de novo jogador.
/// </summary>
public class RegisterDto
{
    /// <summary>
    /// Username único do jogador.
    /// </summary>
    [Required(ErrorMessage = "Username é obrigatório")]
    [MinLength(3, ErrorMessage = "Username deve ter no mínimo 3 caracteres")]
    [MaxLength(50, ErrorMessage = "Username deve ter no máximo 50 caracteres")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Email único do jogador.
    /// </summary>
    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Senha em texto plano (será hasheada antes de salvar).
    /// </summary>
    [Required(ErrorMessage = "Senha é obrigatória")]
    [MinLength(6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Confirmação de senha (validação extra de UX).
    /// Por que no DTO? Porque é validação de input, não regra de negócio.
    /// </summary>
    [Required(ErrorMessage = "Confirmação de senha é obrigatória")]
    [Compare(nameof(Password), ErrorMessage = "Senhas não conferem")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
