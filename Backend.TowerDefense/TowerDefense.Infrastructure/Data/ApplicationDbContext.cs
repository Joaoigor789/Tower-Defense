using Microsoft.EntityFrameworkCore;
using TowerDefense.Domain.Entities;

namespace TowerDefense.Infrastructure.Data;

/// <summary>
/// DbContext do Entity Framework Core.
/// 
/// Por que DbContext?
/// - É o ponto central de acesso ao banco de dados no EF Core.
/// - Gerencia conexões, tracking de entities, e execução de queries.
/// - Permite configurar o schema do banco via Fluent API.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets representam tabelas no banco de dados
    public DbSet<Player> Players { get; set; } = null!;
    public DbSet<TowerMetadata> TowerMetadata { get; set; } = null!;

    /// <summary>
    /// Configuração do modelo usando Fluent API.
    /// Por que Fluent API ao invés de Data Annotations?
    /// - Separação de responsabilidades: Domain fica limpo, sem atributos de infraestrutura.
    /// - Mais poder: Fluent API tem mais opções que Data Annotations.
    /// - Centralização: Toda configuração de banco em um só lugar.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ========================================
        // Configuração da tabela Players
        // ========================================
        modelBuilder.Entity<Player>(entity =>
        {
            // Nome da tabela
            entity.ToTable("Players");

            // Primary Key
            entity.HasKey(p => p.Id);

            // Username: obrigatório, único, max 50 caracteres
            entity.Property(p => p.Username)
                .IsRequired()
                .HasMaxLength(50);
            
            entity.HasIndex(p => p.Username)
                .IsUnique(); // Garante unicidade no banco

            // Email: obrigatório, único, max 100 caracteres
            entity.Property(p => p.Email)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.HasIndex(p => p.Email)
                .IsUnique();

            // PasswordHash: obrigatório, max 255 caracteres
            // Por que 255? BCrypt gera hashes de ~60 chars, mas deixo margem.
            entity.Property(p => p.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            // HighScore: obrigatório, default 0
            entity.Property(p => p.HighScore)
                .IsRequired()
                .HasDefaultValue(0);

            // CreatedAt: obrigatório, default CURRENT_TIMESTAMP
            entity.Property(p => p.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // LastLoginAt: opcional (nullable)
            entity.Property(p => p.LastLoginAt)
                .IsRequired(false);

            // Índice no HighScore para otimizar queries do Leaderboard
            // Por que? Porque vamos fazer ORDER BY HighScore DESC frequentemente.
            entity.HasIndex(p => p.HighScore);
        });

        // ========================================
        // Configuração da tabela TowerMetadata
        // ========================================
        modelBuilder.Entity<TowerMetadata>(entity =>
        {
            entity.ToTable("TowerMetadata");

            entity.HasKey(t => t.Id);

            entity.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(t => t.Damage)
                .IsRequired();

            entity.Property(t => t.Range)
                .IsRequired()
                .HasColumnType("decimal(10,2)"); // Precisão para floats

            entity.Property(t => t.AttackSpeed)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            entity.Property(t => t.Cost)
                .IsRequired();

            entity.Property(t => t.SpriteUrl)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(t => t.AttackSoundUrl)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(t => t.TowerType)
                .IsRequired()
                .HasMaxLength(50);

            // Índice no TowerType para filtros
            entity.HasIndex(t => t.TowerType);
        });

        // ========================================
        // Seed Data (Dados iniciais)
        // ========================================
        // Vou adicionar algumas torres de exemplo para facilitar testes
        modelBuilder.Entity<TowerMetadata>().HasData(
            TowerMetadata.Create(
                "Archer Tower",
                "Torre básica que atira flechas. Bom contra inimigos leves.",
                10, // Damage
                5.0f, // Range
                1.5f, // AttackSpeed
                100, // Cost
                "/assets/sprites/archer-tower.png",
                "/assets/sounds/arrow-shot.mp3",
                "Physical"
            ),
            TowerMetadata.Create(
                "Cannon Tower",
                "Torre pesada com alto dano. Lenta mas poderosa.",
                50,
                6.0f,
                0.5f,
                300,
                "/assets/sprites/cannon-tower.png",
                "/assets/sounds/cannon-boom.mp3",
                "Physical"
            ),
            TowerMetadata.Create(
                "Magic Tower",
                "Torre mágica com dano em área. Efetiva contra grupos.",
                30,
                7.0f,
                1.0f,
                250,
                "/assets/sprites/magic-tower.png",
                "/assets/sounds/magic-blast.mp3",
                "Magic"
            )
        );
    }
}
