namespace TowerDefense.Domain.Entities;

/// <summary>
/// Representa os metadados de uma torre no jogo.
/// Esta entity é usada pela Assets API para retornar informações em JSON
/// ao frontend (ao invés de servir arquivos binários de sprites/sons).
/// 
/// Por que metadados ao invés de arquivos?
/// - Separação de responsabilidades: Backend fornece dados, CDN/Frontend serve assets
/// - Performance: JSON é muito mais leve que binários
/// - Escalabilidade: Podemos usar CDN para sprites/sons
/// </summary>
public class TowerMetadata
{
    public Guid Id { get; private set; }
    
    /// <summary>
    /// Nome da torre (ex: "Archer Tower", "Cannon Tower", "Magic Tower")
    /// </summary>
    public string Name { get; private set; }
    
    /// <summary>
    /// Descrição da torre para exibir na UI do jogo
    /// </summary>
    public string Description { get; private set; }
    
    /// <summary>
    /// Dano base da torre (usado pelo game engine no frontend)
    /// </summary>
    public int Damage { get; private set; }
    
    /// <summary>
    /// Alcance da torre em unidades do jogo
    /// </summary>
    public float Range { get; private set; }
    
    /// <summary>
    /// Taxa de ataque (ataques por segundo)
    /// </summary>
    public float AttackSpeed { get; private set; }
    
    /// <summary>
    /// Custo em moedas do jogo para construir esta torre
    /// </summary>
    public int Cost { get; private set; }
    
    /// <summary>
    /// URL do sprite da torre (pode ser CDN, S3, ou caminho relativo)
    /// Exemplo: "https://cdn.towerdefense.com/sprites/archer-tower.png"
    /// ou "/assets/sprites/archer-tower.png" (para servir do frontend)
    /// </summary>
    public string SpriteUrl { get; private set; }
    
    /// <summary>
    /// URL do som de ataque da torre
    /// Exemplo: "https://cdn.towerdefense.com/sounds/arrow-shot.mp3"
    /// </summary>
    public string AttackSoundUrl { get; private set; }
    
    /// <summary>
    /// Tipo da torre (usado para categorização e filtros)
    /// Exemplos: "Physical", "Magic", "Support"
    /// </summary>
    public string TowerType { get; private set; }

    private TowerMetadata() { }

    /// <summary>
    /// Factory Method para criar metadados de torre.
    /// Por que tantos parâmetros? Porque queremos garantir que todos os dados
    /// necessários sejam fornecidos na criação (fail-fast).
    /// </summary>
    public static TowerMetadata Create(
        string name,
        string description,
        int damage,
        float range,
        float attackSpeed,
        int cost,
        string spriteUrl,
        string attackSoundUrl,
        string towerType)
    {
        // Validações básicas
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome da torre não pode ser vazio", nameof(name));
        
        if (damage < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(damage), 
                "Tower damage cannot be negative.");
        }

        if (range <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(range), 
                "Tower range must be greater than zero. A tower with 0 range is useless.");
        }

        if (attackSpeed <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(attackSpeed), 
                "Attack speed must be greater than zero.");
        }

        if (cost < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(cost), 
                "Tower cost cannot be negative.");
        }

        return new TowerMetadata
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            Damage = damage,
            Range = range,
            AttackSpeed = attackSpeed,
            Cost = cost,
            SpriteUrl = spriteUrl,
            AttackSoundUrl = attackSoundUrl,
            TowerType = towerType
        };
    }

    /// <summary>
    /// Atualiza as estatísticas da torre (útil para balanceamento de jogo).
    /// Por que método ao invés de setters? Para manter o encapsulamento e
    /// permitir validações/regras de negócio futuras.
    /// </summary>
    public void UpdateStats(int damage, float range, float attackSpeed, int cost)
    {
        if (damage < 0 || range <= 0 || attackSpeed <= 0 || cost < 0)
            throw new ArgumentException("Estatísticas inválidas");

        Damage = damage;
        Range = range;
        AttackSpeed = attackSpeed;
        Cost = cost;
    }
}
