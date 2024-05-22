using System.Net;
using System.Net.Sockets;
using GServer.Common;
using GServer.Common.Networking.Messages;

namespace GServer.Server;

public class TCPGameServer : IDisposable
{
    private readonly TcpListener _tcpListener;
    private readonly IPEndPoint _endPoint;
    private readonly GameServerOptions _options;
    private readonly IMessageHandler _messageHandler;

    public TCPGameServer(IPEndPoint endPoint, GameServerOptions options)
    {
        _endPoint = endPoint;
        _options = options;

        _tcpListener = new TcpListener(endPoint);
        _messageHandler = new TCPMessageHandler();
    }

    /// <summary>
    /// Bind the server to the given endpoint.
    /// </summary>
    public async Task Start(CancellationToken cancellationToken)
    {
        Console.WriteLine("Starting TCPGameServer listener...");
        _tcpListener.Start();
        _ = cancellationToken.Register(_tcpListener.Stop);
        Console.WriteLine($"TCPGameServer listening on {_endPoint}");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                using TcpClient tcpClient = await _tcpListener.AcceptTcpClientAsync(cancellationToken);

                await ProcessAsync(tcpClient);

            }
            catch (SocketException) when (cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine("TcpListener stopped listening because cancellation was requested.");
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

            byte[] data = new byte[tcpClient.ReceiveBufferSize];
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
            await _messageHandler.HandleMessageAsync(stream.Socket, new MessageMemoryStream(data));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}