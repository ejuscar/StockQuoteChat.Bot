using StockQuoteChat.Bot.Services;
using StockQuoteChat.Bot.Services.Interfaces;
using StockQuoteChat.Bot;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();

        //IoC
        services.AddSingleton<IMessageManager, RabbitMQManager>();
        services.AddSingleton<IBotService, BotService>();
    })
    .Build();

await host.RunAsync();