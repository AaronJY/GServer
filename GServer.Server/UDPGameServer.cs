using System.Net;
using System.Net.Sockets;
using GServer.Common;
using GServer.Common.Networking.Enums;

namespace GServer.Server;

public class UDPGameServer : IDisposable
{
    public readonly UdpClient UdpClient;

    private readonly IPEndPoint _endPoint;

    public UDPGameServer(IPEndPoint endPoint)
    {
        _endPoint = endPoint;

        UdpClient = new UdpClient();
        UdpClient.Client.SetSocketOption(
            SocketOptionLevel.Socket,
            SocketOptionName.ReuseAddress,
            true);
    }

    /// <summary>
    /// Bind the server to the given endpoint.
    /// </summary>
    public void Start()
    {
        UdpClient.Client.Bind(_endPoint);
        Console.WriteLine($"UDPGameServer listening on {_endPoint}");
    }

    /// <summary>
    /// Begin processing messages
    /// </summary>
    /// <returns></returns>
    public async Task ProcessAsync()
    {
        try
        {
            UdpReceiveResult res = await UdpClient.ReceiveAsync();
            byte[] bytes = res.Buffer;
            MessageMemoryStream stream = new(bytes);
            await HandleMessageAsync(stream, res.RemoteEndPoint);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    private async Task HandleMessageAsync(MessageMemoryStream stream, IPEndPoint remoteEndPoint)
    {
        byte serverPacketInByte = (byte)stream.ReadByte();
        ServerPacketIn serverPacketIn = (ServerPacketIn)serverPacketInByte;

        Console.WriteLine($"Handling UDP message {serverPacketInByte} from {remoteEndPoint}...");

        throw serverPacketIn switch
        {
            ServerPacketIn.AUTH => new NotImplementedException(),
            ServerPacketIn.LIST_SERVERS => new NotImplementedException(),
            _ => new NotImplementedException($"Received unsupported packet {serverPacketInByte}"),
        };
    }

    public void Dispose()
    {
        UdpClient.Close();
        UdpClient.Dispose();
        GC.SuppressFinalize(this);
    }
}