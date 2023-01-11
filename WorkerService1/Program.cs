using StockQuoteChat.Bot.Services;
using StockQuoteChat.Bot.Services.Interfaces;
using WorkerService1;

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