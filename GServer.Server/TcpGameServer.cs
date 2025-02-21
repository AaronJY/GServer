using System.Buffers;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using GServer.Common.Networking.Core;

namespace GServer.Server;

public interface ITcpGameServer : IDisposable
{
    /// <summary>
    /// Bind the server to the given endpoint.
    /// </summary>
    Task StartAsync();
}

public class TcpGameServer(
    IPEndPoint endPoint,
    ITcpMessageHandler messageHandler
) : ITcpGameServer
{
    private readonly TcpListener _tcpListener = new(endPoint);
    private readonly ConcurrentDictionary<TcpClient, ClientState> _clients = new();
    private bool _disposed;
   
    public async Task StartAsync()
    {
        _tcpListener.Start();
        Console.WriteLine($"{nameof(TcpGameServer)} listening on {endPoint}...");

        while (!_disposed)
        {
            try
            {
                TcpClient client = await _tcpListener.AcceptTcpClientAsync();
                Console.WriteLine($"Client accepted: {client.Client.RemoteEndPoint}");
                
                ClientState clientState = new(client);
                _clients.TryAdd(client, clientState);
                
                // Handle client asynchronously using the thread pool
                _ = Task.Run(() => HandleClientAsync(client, clientState));
            }
            catch (Exception ex) when (!_disposed)
            {
                Console.WriteLine($"Error accepting client: {ex.Message}");
            }
        }
    }

    private async Task HandleClientAsync(TcpClient client, ClientState state)
    {
        Console.WriteLine($"Processing request from client: {client.Client.RemoteEndPoint} (Player Id = {state.PlayerId}, Username = {state.Username})");
        
        try
        {
            using (client)
            await using (NetworkStream stream = client.GetStream())
            {
                byte[] buffer = ArrayPool<byte>.Shared.Rent(client.ReceiveBufferSize);

                try
                {
                    while (client.Connected)
                    {
                        // TODO: support cancellation tokens
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                        if (bytesRead == 0)
                            break; // Client disconnected

                        await messageHandler.HandleMessageAsync(stream.Socket, new MessageMemoryStream(buffer), state);
                        
                        state.LastHeartbeat = DateTime.UtcNow;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error occured reading buffer: {e.Message}");
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling client: {ex.Message}");
        }
        finally
        {
            _clients.TryRemove(client, out _);
            Console.WriteLine($"Client disconnected: {client.Client.RemoteEndPoint}");
        }
    }

    private void Stop()
    {
        if (_disposed)
            return;
        
        Console.WriteLine($"Stopping ${nameof(TcpGameServer)} listener...");
        
        // Stop listening for new TCP connections
        _tcpListener.Stop();
        // Disconnect all connected clients
        foreach (TcpClient client in _clients.Keys)
        {
            client.Close();
        }
        // Stop tracking all clients
        _clients.Clear();
        
        Console.WriteLine($"Stopped ${nameof(TcpGameServer)} listener.");
    }

    public void Dispose()
    {
        if (_disposed)
            return;
        
        _disposed = true;
        Stop();
        GC.SuppressFinalize(this);
    }
}