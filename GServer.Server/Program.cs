using System.Net;

namespace GServer.Server;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class Program
{
    private const int ListenPort = 11000;

    private static void Main(string[] args)
    {
        
        CancellationTokenSource cancellationTokenSource = new();

        Thread serverWorker = new(delegate()
        {
            TcpGameServer server = new(new IPEndPoint(IPAddress.Any, ListenPort), new GameServerOptions());
            server.Start();
        });

        serverWorker.Start();
    }
}