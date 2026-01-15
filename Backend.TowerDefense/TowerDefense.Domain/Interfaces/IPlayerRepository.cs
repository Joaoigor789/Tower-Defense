using TowerDefense.Domain.Entities;

namespace TowerDefense.Domain.Interfaces;

/// <summary>
/// Contrato do Repository Pattern para a entidade Player.
/// 
/// Por que interface no Domain?
/// - Inversão de Dependência (SOLID): O Domain define o contrato,
///   a Infrastructure implementa. Assim o Domain não depende de EF Core.
/// - Testabilidade: Podemos mockar esta interface nos testes unitários.
/// - Flexibilidade: Podemos trocar o EF Core por Dapper, MongoDB, etc.
///   sem alterar o Domain ou Application.
/// </summary>
public interface IPlayerRepository
{
    /// <summary>
    /// Busca um Player pelo ID.
    /// Retorna null se não encontrado (ao invés de exception).
    /// Por que Task? Porque operações de I/O devem ser assíncronas.
    /// </summary>
    Task<Player?> GetByIdAsync(Guid id);

    /// <summary>
    /// Busca um Player pelo Username.
    /// Usado no processo de login.
    /// </summary>
    Task<Player?> GetByUsernameAsync(string username);

    /// <summary>
    /// Busca um Player pelo Email.
    /// Usado para validar se o email já está em uso no registro.
    /// </summary>
    Task<Player?> GetByEmailAsync(string email);

    /// <summary>
    /// Retorna os Top N jogadores ordenados por HighScore (decrescente).
    /// Usado para o Leaderboard.
    /// </summary>
    Task<IEnumerable<Player>> GetTopPlayersByScoreAsync(int count);

    /// <summary>
    /// Adiciona um novo Player ao banco de dados.
    /// Por que void? Porque o Player já tem seu Id gerado (Guid.NewGuid()).
    /// A operação de Add apenas marca a entity para inserção.
    /// </summary>
    Task AddAsync(Player player);

    /// <summary>
    /// Atualiza um Player existente.
    /// Usado quando o jogador atualiza seu HighScore ou faz login.
    /// </summary>
    Task UpdateAsync(Player player);

    /// <summary>
    /// Deleta um Player (soft delete ou hard delete, dependendo da implementação).
    /// Incluído para completude, mas pode não ser usado no MVP.
    /// </summary>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Salva todas as mudanças pendentes no banco de dados.
    /// Por que separado? Porque seguimos o padrão Unit of Work.
    /// Podemos fazer múltiplas operações e commitar tudo de uma vez.
    /// </summary>
    Task<int> SaveChangesAsync();
}
