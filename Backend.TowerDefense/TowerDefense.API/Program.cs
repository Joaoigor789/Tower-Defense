using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TowerDefense.Application.Interfaces;
using TowerDefense.Application.Services;
using TowerDefense.Domain.Interfaces;
using TowerDefense.Infrastructure.Auth;
using TowerDefense.Infrastructure.Data;
using TowerDefense.Infrastructure.Repositories;
using TowerDefense.API.Hubs;

var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// CONFIGURAÇÃO DE SERVIÇOS (Dependency Injection)
// ============================================================================

// 1. Configurar DbContext com PostgreSQL
// Por que PostgreSQL? Porque é robusto, open-source, e tem excelente suporte no .NET.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' não encontrada");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// 2. Configurar JWT Authentication
// Por que JWT? Stateless, escalável, e padrão da indústria.
var jwtSecretKey = builder.Configuration["Jwt:SecretKey"]
    ?? throw new InvalidOperationException("Jwt:SecretKey não configurado");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "TowerDefenseAPI",
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "TowerDefenseClient",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            // Mapear 'sub' claim para NameIdentifier
            NameClaimType = "sub"
        };

        // Configuração para SignalR (JWT via query string)
        // Por que? Porque WebSockets não suportam headers customizados.
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                // Se a request é para o Hub e tem token na query string
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// 3. Registrar Repositories (Scoped = uma instância por request HTTP)
// Por que Scoped? Porque o DbContext é Scoped, e repositories dependem dele.
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<ITowerMetadataRepository, TowerMetadataRepository>();

// 4. Registrar Services (Application Layer)
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<LeaderboardService>();
builder.Services.AddScoped<AssetsService>();

// 5. Registrar JWT Token Generator (Singleton = uma instância para toda a aplicação)
// Por que Singleton? Porque não tem estado mutável e pode ser reutilizado.
builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

// 6. Adicionar Controllers
builder.Services.AddControllers();

// 7. Configurar SignalR
builder.Services.AddSignalR();

// 8. Health Checks (Observabilidade)
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>("PostgreSQL");

// 9. Configurar Swagger/OpenAPI (documentação automática da API)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Tower Defense API",
        Version = "v1",
        Description = "Backend API para o jogo Tower Defense - Clean Architecture com .NET 8",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Pamela Menezes",
            Url = new Uri("https://github.com/Joaoigor789/Tower-Defense")
        }
    });

    // Configurar autenticação JWT no Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando Bearer scheme. Exemplo: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// 9. Configurar CORS (permitir requests do frontend)
// Por que CORS? Porque o frontend vai rodar em um domínio diferente (ex: localhost:3000).
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000", // React dev server
                "http://localhost:5173", // Vite dev server
                "https://yourgame.com"   // Produção (ajustar conforme necessário)
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials(); // Necessário para SignalR
    });
});

var app = builder.Build();

// ============================================================================
// CONFIGURAÇÃO DO PIPELINE DE MIDDLEWARE
// ============================================================================

// 1. Swagger (apenas em Development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Tower Defense API v1");
        options.RoutePrefix = string.Empty; // Swagger na raiz (http://localhost:5000/)
    });
}

// 2. HTTPS Redirection (força HTTPS em produção)
app.UseHttpsRedirection();

// 3. CORS (deve vir ANTES de Authentication/Authorization)
app.UseCors("AllowFrontend");

// 4. Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// 5. Mapear Controllers
app.MapControllers();

// 6. Mapear SignalR Hub
app.MapHub<GameHub>("/hubs/game");

// 7. Health Checks
app.MapHealthChecks("/health");

// ============================================================================
// EXECUTAR A APLICAÇÃO
// ============================================================================

app.Run();
