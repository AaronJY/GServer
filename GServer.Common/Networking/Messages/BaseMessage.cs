namespace GServer.Common.Networking.Messages;

public abstract class BaseMessage
{
    protected readonly byte PacketId;

    protected BaseMessage(byte packetId)
    {
        PacketId = packetId;
    }
}