using Microsoft.AspNetCore.SignalR.Client;
using StockQuoteChat.Bot.Models;

namespace StockQuoteChat.Bot.Services.Interfaces
{
    public interface IMessageManager
    {
        void SendMessage<T>(T message);
        void ReceiveMessage(HubConnection hubConnection, UserConnection botConnection);
    }
}
