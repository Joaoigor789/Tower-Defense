using Microsoft.EntityFrameworkCore;
using TowerDefense.Domain.Entities;
using TowerDefense.Domain.Interfaces;
using TowerDefense.Infrastructure.Data;

namespace TowerDefense.Infrastructure.Repositories;

/// <summary>
/// Implementação concreta do ITowerMetadataRepository usando EF Core.
/// </summary>
public class TowerMetadataRepository : ITowerMetadataRepository
{
    private readonly ApplicationDbContext _context;

    public TowerMetadataRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retorna todos os metadados de torres.
    /// AsNoTracking porque é read-only (Assets API).
    /// </summary>
    public async Task<IEnumerable<TowerMetadata>> GetAllAsync()
    {
        return await _context.TowerMetadata
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<TowerMetadata?> GetByIdAsync(Guid id)
    {
        return await _context.TowerMetadata
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<TowerMetadata>> GetByTypeAsync(string towerType)
    {
        return await _context.TowerMetadata
            .AsNoTracking()
            .Where(t => t.TowerType == towerType)
            .ToListAsync();
    }

    public async Task AddAsync(TowerMetadata towerMetadata)
    {
        await _context.TowerMetadata.AddAsync(towerMetadata);
    }

    public async Task UpdateAsync(TowerMetadata towerMetadata)
    {
        _context.TowerMetadata.Update(towerMetadata);
    }

    public async Task DeleteAsync(Guid id)
    {
        var tower = await GetByIdAsync(id);
        if (tower != null)
        {
            _context.TowerMetadata.Remove(tower);
        }
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
