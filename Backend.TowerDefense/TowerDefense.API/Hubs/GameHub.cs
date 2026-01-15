using Microsoft.AspNetCore.SignalR;

namespace TowerDefense.API.Hubs;

/// <summary>
/// SignalR Hub para comunicação real-time entre servidor e clientes.
/// 
/// Por que SignalR?
/// - Real-time: Comunicação bidirecional instantânea (WebSockets).
/// - Escalável: Suporta milhares de conexões simultâneas.
/// - Fácil de usar: Abstração de alto nível sobre WebSockets.
/// 
/// Use Cases futuros:
/// - Multiplayer: Sincronizar estado do jogo entre jogadores.
/// - Chat: Sistema de chat in-game.
/// - Notificações: Alertas em tempo real (ex: "Você foi ultrapassado no ranking!").
/// </summary>
public class GameHub : Hub
{
    private readonly ILogger<GameHub> _logger;

    public GameHub(ILogger<GameHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Chamado quando um cliente se conecta ao Hub.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var connectionId = Context.ConnectionId;
        _logger.LogInformation("Cliente conectado: {ConnectionId}", connectionId);

        // Enviar mensagem de boas-vindas para o cliente
        await Clients.Caller.SendAsync("Connected", new
        {
            message = "Conectado ao GameHub com sucesso!",
            connectionId
        });

        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Chamado quando um cliente se desconecta do Hub.
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionId = Context.ConnectionId;
        
        if (exception != null)
        {
            _logger.LogError(exception, "Cliente desconectado com erro: {ConnectionId}", connectionId);
        }
        else
        {
            _logger.LogInformation("Cliente desconectado: {ConnectionId}", connectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Método de exemplo: Broadcast de mensagem para todos os clientes conectados.
    /// O frontend pode chamar: connection.invoke("BroadcastMessage", "Olá, mundo!");
    /// </summary>
    public async Task BroadcastMessage(string message)
    {
        _logger.LogInformation("Broadcast de mensagem: {Message}", message);

        // Enviar para TODOS os clientes conectados
        await Clients.All.SendAsync("ReceiveMessage", new
        {
            message,
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Método de exemplo: Enviar mensagem para um cliente específico.
    /// Útil para comunicação 1-to-1 (ex: convites de partida).
    /// </summary>
    public async Task SendMessageToUser(string connectionId, string message)
    {
        _logger.LogInformation("Enviando mensagem para {ConnectionId}: {Message}", connectionId, message);

        await Clients.Client(connectionId).SendAsync("ReceiveMessage", new
        {
            message,
            from = Context.ConnectionId,
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Método de exemplo: Entrar em uma "sala" (grupo).
    /// Útil para partidas multiplayer (cada partida = uma sala).
    /// </summary>
    public async Task JoinRoom(string roomName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        
        _logger.LogInformation("Cliente {ConnectionId} entrou na sala {RoomName}", Context.ConnectionId, roomName);

        // Notificar todos na sala
        await Clients.Group(roomName).SendAsync("UserJoinedRoom", new
        {
            connectionId = Context.ConnectionId,
            roomName,
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Método de exemplo: Sair de uma "sala" (grupo).
    /// </summary>
    public async Task LeaveRoom(string roomName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        
        _logger.LogInformation("Cliente {ConnectionId} saiu da sala {RoomName}", Context.ConnectionId, roomName);

        // Notificar todos na sala
        await Clients.Group(roomName).SendAsync("UserLeftRoom", new
        {
            connectionId = Context.ConnectionId,
            roomName,
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Método de exemplo: Sincronizar estado do jogo em uma sala.
    /// O frontend pode enviar o estado atual do jogo e todos na sala recebem.
    /// </summary>
    public async Task SyncGameState(string roomName, object gameState)
    {
        _logger.LogInformation("Sincronizando estado do jogo na sala {RoomName}", roomName);

        // Enviar para todos na sala EXCETO o remetente
        await Clients.OthersInGroup(roomName).SendAsync("GameStateUpdated", new
        {
            gameState,
            from = Context.ConnectionId,
            timestamp = DateTime.UtcNow
        });
    }
}
