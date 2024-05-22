using System.Net;
using GServer.Server;

internal sealed class Program
{
    private const int LISTEN_PORT = 11000;

    private static void Main(string[] args)
    {
        TCPGameServer server = new(new IPEndPoint(IPAddress.Any, LISTEN_PORT), new GameServerOptions());
        CancellationTokenSource cancellationTokenSource = new();

        server.Start(cancellationTokenSource.Token).Wait();
    }
}