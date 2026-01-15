using Microsoft.AspNetCore.Mvc;
using TowerDefense.Application.DTOs.Assets;
using TowerDefense.Application.Services;

namespace TowerDefense.API.Controllers;

/// <summary>
/// Controller responsável por fornecer metadados de assets (torres, sprites, sons).
/// 
/// Por que Assets API ao invés de servir arquivos?
/// - Separação de responsabilidades: Backend fornece dados, CDN/Frontend serve binários.
/// - Performance: JSON é muito mais leve que arquivos de imagem/som.
/// - Escalabilidade: Podemos usar CDN para assets estáticos.
/// - Flexibilidade: Podemos mudar URLs de assets sem alterar o código do jogo.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AssetsController : ControllerBase
{
    private readonly AssetsService _assetsService;
    private readonly ILogger<AssetsController> _logger;

    public AssetsController(
        AssetsService assetsService,
        ILogger<AssetsController> logger)
    {
        _assetsService = assetsService;
        _logger = logger;
    }

    /// <summary>
    /// Retorna metadados de todas as torres disponíveis.
    /// GET /api/assets/towers
    /// 
    /// O frontend usa isso para:
    /// - Renderizar o menu de construção de torres
    /// - Saber as stats de cada torre (Damage, Range, etc.)
    /// - Carregar sprites e sons dinamicamente
    /// </summary>
    [HttpGet("towers")]
    [ProducesResponseType(typeof(IEnumerable<TowerMetadataDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllTowers()
    {
        try
        {
            _logger.LogInformation("Requisição para listar todas as torres");

            var towers = await _assetsService.GetAllTowersAsync();

            return Ok(towers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar torres");
            return StatusCode(500, new { error = "Erro ao buscar torres" });
        }
    }

    /// <summary>
    /// Retorna metadados de torres filtradas por tipo.
    /// GET /api/assets/towers/type/{towerType}
    /// 
    /// Exemplo: GET /api/assets/towers/type/Physical
    /// </summary>
    [HttpGet("towers/type/{towerType}")]
    [ProducesResponseType(typeof(IEnumerable<TowerMetadataDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTowersByType(string towerType)
    {
        try
        {
            _logger.LogInformation("Requisição para listar torres do tipo: {TowerType}", towerType);

            var towers = await _assetsService.GetTowersByTypeAsync(towerType);

            return Ok(towers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar torres por tipo");
            return StatusCode(500, new { error = "Erro ao buscar torres" });
        }
    }
}
