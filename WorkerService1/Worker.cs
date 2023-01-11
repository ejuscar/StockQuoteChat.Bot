using Microsoft.AspNetCore.SignalR.Client;
using StockQuoteChat.Bot.Models;
using StockQuoteChat.Bot.Services.Interfaces;

namespace StockQuoteChat.Bot
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IBotService _botService;
        private readonly IMessageManager _messageManager;
        private readonly string _commandMessage;

        public Worker(ILogger<Worker> logger, IBotService botService, IMessageManager messageManager)
        {
            _logger = logger;
            _commandMessage = "/stock=";
            _botService = botService;
            _messageManager = messageManager;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var response = await _botService.Authenticate();

            if (!response.Success)
            {
                _logger.LogError($"Failed to login bot to api. Error: {response.Error}");
                await this.StopAsync(stoppingToken);
            }

            HubConnection hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7173/chat", options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(response.Data.Token);
                })
                .Build();

            var botConnection = new UserConnection(
                    new User(response.Data!),
                    new Room());

            #region Handlers
            // Displays a message in chat room saying that user has joined the room
            hubConnection.On<UserConnection>("UserJoined", async userConnection =>
            {
                botConnection.Room = userConnection.Room;
                await hubConnection.InvokeAsync("AddUserToRoom", botConnection);
                await hubConnection.InvokeAsync("SendBotMessage", $"{userConnection.User.GetUserName()} has joined {userConnection.Room.Name}", botConnection, true);
            });

            // Displays a message in chat room saying that user has left the room
            hubConnection.On<UserConnection>("UserLeft", userConnection =>
            {
                botConnection.Room = userConnection.Room;
                hubConnection.InvokeAsync("SendBotMessage", $"{userConnection.User.GetUserName()} has left", botConnection, true);
            });

            // Process command message
            hubConnection.On<string, UserConnection>("ReceiveCommand", (message, userConnection) =>
            {
                _logger.LogTrace($"Processing command. Message: {message}");

                // Get the stockCode in the command message
                var stockCode = message.ToLower()
                    .Split(" ")
                    .First(w => w.Contains(_commandMessage))
                    ?.Replace(_commandMessage, "");

                if (!string.IsNullOrEmpty(stockCode))
                {
                    var taskIsQueued = _botService.ProcessMessage(stockCode!.Trim(), userConnection.Room);
                    
                    if (!taskIsQueued.Result)
                        hubConnection.InvokeAsync("SendBotMessage", $"Unrecognized stock code: {stockCode.Trim()}", botConnection, false);
                }
                else
                {
                    botConnection.Room = userConnection.Room;
                    hubConnection.InvokeAsync("SendBotMessage", "Stock Code cannot be empty.", botConnection, false);
                }

                _logger.LogTrace("Command processing finished");

            });
            #endregion

            try
            {
                await hubConnection.StartAsync();
                _logger.LogInformation("Hub connected successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to connect to hub. Error: {ex.Message}");
                await StopAsync(stoppingToken);
            }

            await hubConnection.InvokeAsync("AddBotConnection", botConnection);

            while (!stoppingToken.IsCancellationRequested)
            {
                _messageManager.ReceiveMessage(hubConnection, botConnection);
                await Task.Delay(5000, stoppingToken);
            }
        }

    }
}