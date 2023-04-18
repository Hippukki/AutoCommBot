using AutoCommBot;
using AutoCommBot.Bots;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHttpClient();
        services.AddHostedService<Worker>();
        services.AddTransient(typeof(CommentBot));
    })
    .Build();

await host.RunAsync();
