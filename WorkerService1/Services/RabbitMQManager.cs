using Microsoft.AspNetCore.SignalR.Client;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StockQuoteChat.Bot.Models;
using StockQuoteChat.Bot.Services.Interfaces;
using System.Text;
using System.Text.Json;

namespace StockQuoteChat.Bot.Services
{
    public class RabbitMQManager : IMessageManager
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQManager()
        {
            _connectionFactory = new ConnectionFactory { HostName = "localhost" };
            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "stock_quote_chat",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
        }

        public void SendMessage<T>(T message)
        {
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);
            _channel.BasicPublish(exchange: "", routingKey: "stock_quote_chat", body: body);
        }

        public void ReceiveMessage(HubConnection hubConnection, UserConnection botConnection)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var parsedBody = JsonSerializer.Deserialize<QueueMessage>(Encoding.UTF8.GetString(body), new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                botConnection.Room = parsedBody.Room;
                hubConnection.InvokeAsync("SendBotMessage", parsedBody.Message, botConnection, false);
            };

            _channel.BasicConsume(queue: "stock_quote_chat",
                                     autoAck: true,
                                     consumer: consumer);
        }
    }
}
