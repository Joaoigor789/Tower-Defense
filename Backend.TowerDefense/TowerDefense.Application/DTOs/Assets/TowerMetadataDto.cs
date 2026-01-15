namespace TowerDefense.Application.DTOs.Assets;

/// <summary>
/// DTO para retornar metadados de torre pela Assets API.
/// 
/// Por que DTO separado da Entity TowerMetadata?
/// - Controle sobre o que é exposto: Posso omitir campos internos (ex: Id).
/// - Versionamento: Posso mudar a Entity sem quebrar a API.
/// - Transformações: Posso formatar dados de forma diferente (ex: URLs absolutas).
/// </summary>
public class TowerMetadataDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Damage { get; set; }
    public float Range { get; set; }
    public float AttackSpeed { get; set; }
    public int Cost { get; set; }
    public string SpriteUrl { get; set; } = string.Empty;
    public string AttackSoundUrl { get; set; } = string.Empty;
    public string TowerType { get; set; } = string.Empty;

    /// <summary>
    /// DPS calculado (Damage Per Second).
    /// Por que aqui? Porque é um dado derivado útil para o frontend,
    /// mas não precisa ser armazenado no banco (pode ser calculado on-the-fly).
    /// </summary>
    public float DamagePerSecond => Damage * AttackSpeed;
}
