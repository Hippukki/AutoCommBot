using AutoCommBot;
using AutoCommBot.Bots;
using AutoCommBot.Providers;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<ITelegramProvider, TelegramProvider>();
        services.AddHttpClient();
        services.AddHostedService<Worker>();
        services.AddTransient(typeof(CommentBot));
    })
    .Build();

await host.RunAsync();
