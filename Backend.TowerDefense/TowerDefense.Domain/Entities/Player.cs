namespace TowerDefense.Domain.Entities;

/// <summary>
/// Representa um jogador no sistema Tower Defense.
/// Esta entity contém toda a lógica de negócio relacionada ao Player,
/// seguindo o princípio de Rich Domain Model (não é apenas um POCO com getters/setters).
/// </summary>
public class Player
{
    public Guid Id { get; private set; }
    public string Username { get; private set; }
    public string Email { get; private set; }
    
    // Armazenamos o hash da senha, NUNCA a senha em texto plano
    // O hash é gerado usando BCrypt na camada de Application
    public string PasswordHash { get; private set; }
    
    public int HighScore { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    // EF Core precisa de um construtor sem parâmetros (pode ser private)
    private Player() { }

    /// <summary>
    /// Factory Method para criar um novo Player.
    /// Uso: Player.Create("pamela", "pamela@email.com", hashedPassword)
    /// Por que factory method? Garante que o Player sempre seja criado em estado válido.
    /// </summary>
    public static Player Create(string username, string email, string passwordHash)
    {
        // Validações de negócio aqui (não deixamos criar Player inválido)
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username não pode ser vazio", nameof(username));
        
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email não pode ser vazio", nameof(email));
        
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("PasswordHash não pode ser vazio", nameof(passwordHash));

        return new Player
        {
            Id = Guid.NewGuid(),
            Username = username,
            Email = email,
            PasswordHash = passwordHash,
            HighScore = 0,
            CreatedAt = DateTime.UtcNow,
            LastLoginAt = null
        };
    }

    /// <summary>
    /// Atualiza o HighScore do jogador.
    /// Por que método ao invés de setter público? 
    /// Porque podemos adicionar regras de negócio (ex: só atualizar se for maior).
    /// </summary>
    public void UpdateHighScore(int newScore)
    {
        // Regra de negócio: só atualiza se o novo score for maior
        if (newScore > HighScore)
        {
            HighScore = newScore;
        }
    }

    /// <summary>
    /// Registra o último login do jogador.
    /// Útil para analytics e detecção de contas inativas.
    /// </summary>
    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Valida se a senha fornecida corresponde ao hash armazenado.
    /// Por que aqui? Porque a validação de senha é lógica de domínio do Player.
    /// Nota: A verificação real do BCrypt acontece na camada Application,
    /// mas a decisão de "este Player pode autenticar com esta senha" é do Domain.
    /// </summary>
    public bool CanAuthenticateWith(string passwordHash)
    {
        return PasswordHash == passwordHash;
    }
}
