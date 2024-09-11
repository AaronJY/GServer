using System.Net.Sockets;
using NetCoreServer;

namespace GServer.NCSServer;

public class GameSession : TcpSession
{
    public GameSession(TcpServer server) : base(server)
    {
    }

    protected override void OnConnected()
    {
        Console.WriteLine($"Chat TCP session with Id {Id} connected!");
    }

    protected override void OnConnecting()
    {
        base.OnConnecting();
    }

    protected override void OnDisconnecting()
    {
        base.OnDisconnecting();
    }

    protected override void OnDisconnected()
    {
        Console.WriteLine($"Chat TCP session with Id {Id} disconnected!");
    }

    protected override void OnReceived(byte[] buffer, long offset, long size)
    {
        base.OnReceived(buffer, offset, size);
    }

    protected override void OnSent(long sent, long pending)
    {
        base.OnSent(sent, pending);
    }

    protected override void OnEmpty()
    {
        base.OnEmpty();
    }

    protected override void OnError(SocketError error)
    {
        base.OnError(error);
    }
}