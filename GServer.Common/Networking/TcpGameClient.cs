using System.Net.Sockets;

namespace GServer.Common.Networking;

public interface ITcpGameClient
{
    /// <summary>
    /// Connects to the game server.
    /// </summary>
    /// <param name="host">Game server host.</param>
    /// <param name="port">Game server port.</param>
    Task ConnectAsync(string host, int port);
}

public class TcpGameClient : ITcpGameClient, IDisposable
{
    private TcpClient _tcpClient;
    
    public async Task ConnectAsync(string host, int port)
    {
        _tcpClient = new TcpClient();
        _tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        await _tcpClient.ConnectAsync(host, port);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _tcpClient.Dispose();
    }
}