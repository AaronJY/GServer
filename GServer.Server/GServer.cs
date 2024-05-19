using System.Net;
using System.Net.Sockets;
using GServer.Server.Services;

namespace GServer.Server
{
    public class GServer : IDisposable
    {
        public readonly UdpClient UdpClient;
        private IPEndPoint _ipEndpoint;

        private readonly ServerListService _serverListService;

        public GServer(UdpClient udpClient, IPEndPoint ipEndPoint)
        {
            UdpClient = udpClient;
            _ipEndpoint = ipEndPoint;

            UdpClient.Client.SetSocketOption(
                SocketOptionLevel.Socket,
                SocketOptionName.ReuseAddress,
                true);

            _serverListService = new ServerListService();
        }

        public void Bind()
        {
            UdpClient.Client.Bind(_ipEndpoint);
            Console.WriteLine("Now listening on " + UdpClient.Client.LocalEndPoint);
        }

        public void Dispose()
        {
            UdpClient.Close();
            UdpClient.Dispose();
        }
    }
}