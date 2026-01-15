using TowerDefense.Domain.Entities;

namespace TowerDefense.Domain.Interfaces;

/// <summary>
/// Contrato do Repository Pattern para a entidade TowerMetadata.
/// 
/// Por que um repository separado?
/// - Single Responsibility: Cada repository gerencia apenas uma entity.
/// - Queries específicas: TowerMetadata tem queries diferentes de Player
///   (ex: filtrar por tipo, buscar por range, etc.).
/// </summary>
public interface ITowerMetadataRepository
{
    /// <summary>
    /// Retorna todos os metadados de torres disponíveis.
    /// Usado pela Assets API para retornar o catálogo completo de torres.
    /// </summary>
    Task<IEnumerable<TowerMetadata>> GetAllAsync();

    /// <summary>
    /// Busca metadados de uma torre específica pelo ID.
    /// </summary>
    Task<TowerMetadata?> GetByIdAsync(Guid id);

    /// <summary>
    /// Busca metadados de torres por tipo (ex: "Physical", "Magic").
    /// Útil se o frontend quiser filtrar torres por categoria.
    /// </summary>
    Task<IEnumerable<TowerMetadata>> GetByTypeAsync(string towerType);

    /// <summary>
    /// Adiciona novos metadados de torre ao banco.
    /// Usado por um sistema de admin/seeding para popular o catálogo.
    /// </summary>
    Task AddAsync(TowerMetadata towerMetadata);

    /// <summary>
    /// Atualiza metadados existentes (útil para balanceamento de jogo).
    /// </summary>
    Task UpdateAsync(TowerMetadata towerMetadata);

    /// <summary>
    /// Remove metadados de torre (caso uma torre seja descontinuada).
    /// </summary>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Salva todas as mudanças pendentes.
    /// </summary>
    Task<int> SaveChangesAsync();
}
