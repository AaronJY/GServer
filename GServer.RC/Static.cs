using GServer.Common.Networking;

namespace GServer.RC;

public static class Static
{
    public static ITcpGameClient TcpGameClient { get; } = new TcpGameClient();
}