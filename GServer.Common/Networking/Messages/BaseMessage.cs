namespace GServer.Common.Networking.Messages;

public abstract class BaseMessage
{
    protected readonly byte PacketId;

    public BaseMessage(byte packetId)
    {
        PacketId = packetId;
    }
}