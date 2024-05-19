using GServer.Common.Game.Entities;
using GServer.Common.Networking.Enums;

namespace GServer.Common.Networking.Messages.Client;

public class ListServersResponseMessage : BaseMessage, IMessage<ListServersResponseMessage>
{
    public IEnumerable<ServerListing> ServerListings { get; set; }

    public ListServersResponseMessage(IEnumerable<ServerListing> serverListings) : base((byte)ClientPacketIn.LIST_SERVERS_RESPONSE)
    {
        ServerListings = serverListings;
    }

    public ListServersResponseMessage(MessageMemoryStream stream) : base((byte)ClientPacketIn.LIST_SERVERS_RESPONSE)
    {
        throw new NotImplementedException();
    }

    public byte[] Serialize()
    {
        throw new NotImplementedException();
    }
}
