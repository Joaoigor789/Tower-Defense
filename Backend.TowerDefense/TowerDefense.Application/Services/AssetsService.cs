using TowerDefense.Application.DTOs.Assets;
using TowerDefense.Domain.Interfaces;

namespace TowerDefense.Application.Services;

/// <summary>
/// Service responsável por fornecer metadados de assets (torres, sons, sprites).
/// </summary>
public class AssetsService
{
    private readonly ITowerMetadataRepository _towerMetadataRepository;

    public AssetsService(ITowerMetadataRepository towerMetadataRepository)
    {
        _towerMetadataRepository = towerMetadataRepository;
    }

    /// <summary>
    /// Retorna todos os metadados de torres disponíveis.
    /// O frontend usa isso para saber quais torres existem e suas stats.
    /// </summary>
    public async Task<IEnumerable<TowerMetadataDto>> GetAllTowersAsync()
    {
        var towers = await _towerMetadataRepository.GetAllAsync();

        // Transformar entities em DTOs
        var towerDtos = towers.Select(t => new TowerMetadataDto
        {
            Name = t.Name,
            Description = t.Description,
            Damage = t.Damage,
            Range = t.Range,
            AttackSpeed = t.AttackSpeed,
            Cost = t.Cost,
            SpriteUrl = t.SpriteUrl,
            AttackSoundUrl = t.AttackSoundUrl,
            TowerType = t.TowerType
            // DamagePerSecond é calculado automaticamente no DTO
        }).ToList();

        return towerDtos;
    }

    /// <summary>
    /// Retorna metadados de torres filtrados por tipo.
    /// </summary>
    public async Task<IEnumerable<TowerMetadataDto>> GetTowersByTypeAsync(string towerType)
    {
        var towers = await _towerMetadataRepository.GetByTypeAsync(towerType);

        var towerDtos = towers.Select(t => new TowerMetadataDto
        {
            Name = t.Name,
            Description = t.Description,
            Damage = t.Damage,
            Range = t.Range,
            AttackSpeed = t.AttackSpeed,
            Cost = t.Cost,
            SpriteUrl = t.SpriteUrl,
            AttackSoundUrl = t.AttackSoundUrl,
            TowerType = t.TowerType
        }).ToList();

        return towerDtos;
    }
}
