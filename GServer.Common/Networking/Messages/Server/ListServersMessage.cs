using GServer.Common.Networking.Enums;

namespace GServer.Common.Networking.Messages.Server;

public class ListServersMessage : BaseMessage, IMessage<ListServersMessage>
{
    public ListServersMessage() : base((byte)ServerPacketIn.LIST_SERVERS)
    {
    }

    public byte[] Serialize()
    {
        return [];
    }
}
