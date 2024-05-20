using System.Net;
using GServer.Server;

internal class Program
{
    private const int LISTEN_PORT = 11000;


    private static void Main(string[] args)
    {
        UDPGameServer udpGameServer = new(new IPEndPoint(IPAddress.Any, LISTEN_PORT));

        udpGameServer.Start();

        while (true)
        {
            _ = udpGameServer.ProcessAsync();
        }
    }
}