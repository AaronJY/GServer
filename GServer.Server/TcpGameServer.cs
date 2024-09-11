using System.Net;
using System.Net.Sockets;
using GServer.Common.Networking.Core;
using GServer.Common.Networking.Messages;

namespace GServer.Server;

public class TcpGameServer(IPEndPoint endPoint, GameServerOptions options) : IDisposable
{
    private readonly GameServerOptions _options = options;
    private readonly TcpListener _tcpListener = new(endPoint);
    private readonly IMessageHandler _messageHandler = new TcpMessageHandler();

    /// <summary>
    /// Bind the server to the given endpoint.
    /// </summary>
    public void Start()
    {
        Console.WriteLine($"Starting ${nameof(TcpGameServer)} listener...");
        _tcpListener.Start();
        Console.WriteLine($"{nameof(TcpGameServer)} listening on {endPoint}");

        while (true)
        {
            try
            {
                Console.WriteLine("Waiting for a connection...");

                _tcpListener.AcceptTcpClient();
                Console.WriteLine("Client accepted!");

                Thread worker = new Thread(new ParameterizedThreadStart(HandleClient!));
                worker.Start();
            }
            finally
            {
                _tcpListener.Stop();
            }
        }
    }

    private void HandleClient(object clientObj)
    {
        TcpClient tcpClient = (TcpClient)clientObj;

        try
        {
            using NetworkStream stream = tcpClient.GetStream();

            byte[] data = new byte[tcpClient.ReceiveBufferSize];
            while (stream.Read(data, 0, data.Length) != 0)
            {
                // Use the in-memory buffer to process the message
                _messageHandler.HandleMessageAsync(stream.Socket, new MessageMemoryStream(data)).Wait();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    private async void Stop()
    {
        Console.WriteLine($"Stopping ${nameof(TcpGameServer)} listener...");
        _tcpListener.Stop();
        Console.WriteLine($"Stopped ${nameof(TcpGameServer)} listener.");
    }

    public void Dispose()
    {
        Stop();
        GC.SuppressFinalize(this);
    }
}