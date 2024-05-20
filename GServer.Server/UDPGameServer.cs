using System.Net;
using System.Net.Sockets;
using GServer.Common;
using GServer.Common.Networking.Enums;
using GServer.Common.Networking.Messages.Client;
using GServer.Common.Networking.Messages.Server;

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
        ServerPacketIn serverPacketIn = (ServerPacketIn)stream.ReadByte();

        Console.WriteLine($"Handling message {serverPacketIn} from {remoteEndPoint}...");

        switch (serverPacketIn)
        {
            case ServerPacketIn.AUTH:
                AuthMessage msg = new(stream);

                AuthResponseMessage resp = msg.Username == "aaronyarbz" && msg.Password == "password123"
                    ? new(true, Guid.NewGuid().ToString(), null)
                    : new(false, null, AuthResponseFailure.IncorrectLoginOrPassword);

                byte[] buffer = resp.Serialize();
                _ = await UdpClient.SendAsync(buffer, buffer.Length, remoteEndPoint);

                break;

            case ServerPacketIn.LIST_SERVERS:
                throw new NotImplementedException();

            default:
                Console.WriteLine($"Received unsupported packet.");
                break;
        }
    }

    public void Dispose()
    {
        UdpClient.Close();
        UdpClient.Dispose();
        GC.SuppressFinalize(this);
    }
}