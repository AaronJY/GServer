using GServer.Common.Game.Entities;
using GServer.Common.Networking.Core;
using GServer.Common.Networking.Enums;

namespace GServer.Common.Networking.Messages.Server;

public class ListServersMessage : BaseMessage, IMessage<ListServersMessage>
{
    public ServerListing[] ServerListings;

    public const int SERVER_NAME_BYTES = 50;

    public ListServersMessage(ServerListing[] serverListings) : base((byte)ServerPacketIn.ListServers)
    {
        ServerListings = serverListings;
    }

    public byte[] Serialize()
    {
        using MessageMemoryStream stream = new();

        stream.WriteByte(PacketId);

        foreach (ServerListing listing in ServerListings)
        {
            using MessageMemoryStream listingStream = new();

            listingStream.WriteUtf8StringOfSize(50, listing.Name);
            listingStream.WriteUtf8StringOfSize(1000, listing.Description);
            listingStream.WriteUInt16(listing.Playercount);
            listingStream.WriteUtf8StringOfSize(15, listing.IpAddress);
            listingStream.WriteUInt16(listing.Port);
            listingStream.WriteByte((byte)listing.ServerTier);

            long listingBlockSize = listingStream.Length;
            stream.WriteInt64(listingBlockSize);
            stream.WriteBytes(listingStream.ToArray());
        }

        return stream.ToArray();
    }
}
