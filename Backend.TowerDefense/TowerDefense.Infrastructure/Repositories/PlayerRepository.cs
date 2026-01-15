using Microsoft.EntityFrameworkCore;
using TowerDefense.Domain.Entities;
using TowerDefense.Domain.Interfaces;
using TowerDefense.Infrastructure.Data;

namespace TowerDefense.Infrastructure.Repositories;

/// <summary>
/// Implementação concreta do IPlayerRepository usando EF Core.
/// 
/// Por que Repository Pattern?
/// - Abstração: O Domain não sabe que estamos usando EF Core.
/// - Testabilidade: Posso mockar o repository nos testes.
/// - Centralização: Todas as queries de Player em um só lugar.
/// </summary>
public class PlayerRepository : IPlayerRepository
{
    private readonly ApplicationDbContext _context;

    public PlayerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Player?> GetByIdAsync(Guid id)
    {
        return await _context.Players
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Player?> GetByUsernameAsync(string username)
    {
        return await _context.Players
            .FirstOrDefaultAsync(p => p.Username == username);
    }

    public async Task<Player?> GetByEmailAsync(string email)
    {
        return await _context.Players
            .FirstOrDefaultAsync(p => p.Email == email);
    }

    /// <summary>
    /// Retorna os Top N jogadores ordenados por HighScore (decrescente).
    /// Por que AsNoTracking? Porque não vou modificar essas entities,
    /// então não preciso de tracking (performance).
    /// </summary>
    public async Task<IEnumerable<Player>> GetTopPlayersByScoreAsync(int count)
    {
        return await _context.Players
            .AsNoTracking()
            .OrderByDescending(p => p.HighScore)
            .Take(count)
            .ToListAsync();
    }

    public async Task AddAsync(Player player)
    {
        await _context.Players.AddAsync(player);
    }

    public async Task UpdateAsync(Player player)
    {
        _context.Players.Update(player);
    }

    public async Task DeleteAsync(Guid id)
    {
        var player = await GetByIdAsync(id);
        if (player != null)
        {
            _context.Players.Remove(player);
        }
    }

    /// <summary>
    /// Salva todas as mudanças pendentes no banco.
    /// Por que retornar int? Porque SaveChangesAsync retorna o número
    /// de entities afetadas (útil para validações).
    /// </summary>
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
