using System.Net;
using GServer.Server.Business.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GServer.Server;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class Program
{
    private const int ListenPort = 11000;

    private static async Task Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

        // Register services
        _ = builder.Services.AddScoped<IAuthService, AuthService>();
        _ = builder.Services.AddScoped<ITcpMessageHandler, TcpMessageHandler>();
        _ = builder.Services.AddTransient<ITcpGameServer>((services) =>
        {
            return new TcpGameServer(
                new IPEndPoint(IPAddress.Any, ListenPort),
                services.GetRequiredService<ITcpMessageHandler>()
            );
        });

        // Start service
        using IHost host = builder.Build();
        ApplicationLifetime(host.Services);
        await host.RunAsync();
    }

    private static void ApplicationLifetime(IServiceProvider hostProvider)
    {
        using IServiceScope serviceScope = hostProvider.CreateScope();

        Thread serverWorker = new(() =>
        {
            ITcpGameServer server = serviceScope.ServiceProvider.GetRequiredService<ITcpGameServer>();
            server.Start();

            while (true)
            {
                // Sleep to not consume too much CPU while waiting
                Thread.Sleep(1000);
            }
        });

        serverWorker.Start();
        serverWorker.Join(); // Wait for the thread to complete before disposing the scope
    }
}