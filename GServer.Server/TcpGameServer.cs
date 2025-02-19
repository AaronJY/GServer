using System.Net;
using System.Net.Sockets;
using GServer.Common.Networking.Core;

namespace GServer.Server;

public interface ITcpGameServer
{
    void Dispose();
    void Start();
}

public class TcpGameServer(
    IPEndPoint endPoint,
    ITcpMessageHandler messageHandler
) : IDisposable, ITcpGameServer
{
    private readonly TcpListener _tcpListener = new(endPoint);

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

                TcpClient client = _tcpListener.AcceptTcpClient();
                Console.WriteLine("Client accepted!");

                Thread worker = new(new ParameterizedThreadStart(HandleClient!)); // TODO: use thread pools instead
                worker.Start(client);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occured while processing a tcp connection: {ex.Message}");
            }
        }
    }

    private async void HandleClient(object clientObj)
    {
        if (clientObj is not TcpClient tcpClient)
        {
            return;
        }

        try
        {
            using (tcpClient)
            using (NetworkStream stream = tcpClient.GetStream())
            {
                byte[] data = new byte[tcpClient.ReceiveBufferSize];
                while (stream.Read(data, 0, data.Length) != 0)
                {
                    // Use the in-memory buffer to process the message
                    await messageHandler.HandleMessageAsync(stream.Socket, new MessageMemoryStream(data));
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling client: {ex.Message}");
        }
    }

    private void Stop()
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