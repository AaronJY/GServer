using System.Net;
using System.Net.Sockets;
using GServer.Common;
using GServer.Common.Networking.Enums;
using GServer.Common.Networking.Messages.Client;
using GServer.Common.Networking.Messages.Server;

namespace GServer.Server;

public class TCPGameServer : IDisposable
{
    private readonly TcpListener _tcpListener;
    private readonly IPEndPoint _endPoint;
    private readonly GameServerOptions _options;

    public TCPGameServer(IPEndPoint endPoint, GameServerOptions options)
    {
        _endPoint = endPoint;
        _options = options;

        _tcpListener = new TcpListener(endPoint);
    }

    /// <summary>
    /// Bind the server to the given endpoint.
    /// </summary>
    public async void Start()
    {
        Console.WriteLine("Starting TCPGameServer listener...");
        _tcpListener.Start();
        Console.WriteLine($"TCPGameServer listening on {_endPoint}");

        try
        {
            while (true)
            {
                await ProcessAsync(await _tcpListener.AcceptTcpClientAsync());
            }
        }
        catch
        {
            throw;
        }
        finally
        {
            _tcpListener.Stop();
        }
    }

    /// <summary>
    /// Begin processing messages
    /// </summary>
    /// <returns></returns>
    public async Task ProcessAsync(TcpClient tcpClient)
    {
        try
        {
            using NetworkStream stream = tcpClient.GetStream();

            byte[] data = new byte[_options.PacketLength];
            int bytesRead = 0;
            int chunkSize = 1;

            // Read everything into the data buffer
            while (bytesRead < data.Length && chunkSize > 0)
            {
                bytesRead +=
                    chunkSize =
                        await stream.ReadAsync(data.AsMemory(bytesRead, data.Length - bytesRead));
            }

            // Use the in-memory buffer to process the message
            await HandleMessageAsync(stream.Socket, new MessageMemoryStream(data));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    private async Task HandleMessageAsync(Socket clientSocket, MessageMemoryStream messageStream)
    {
        ServerPacketIn serverPacketIn = (ServerPacketIn)messageStream.ReadByte();

        Console.WriteLine($"Handling message {serverPacketIn} from {client}...");

        switch (serverPacketIn)
        {
            case ServerPacketIn.AUTH:
                AuthMessage msg = new(messageStream);

                AuthResponseMessage resp = msg.Username == "aaronyarbz" && msg.Password == "password123"
                    ? new(true, Guid.NewGuid().ToString(), null)
                    : new(false, null, AuthResponseFailure.IncorrectLoginOrPassword);

                byte[] buffer = resp.Serialize();
                _ = await TcpClient.Client.SendAsync(buffer, 

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
        TcpClient.Close();
        TcpClient.Dispose();
        GC.SuppressFinalize(this);
    }
}