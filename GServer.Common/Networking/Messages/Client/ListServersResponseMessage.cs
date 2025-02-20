using GServer.Common.Game.Entities;
using GServer.Common.Networking.Core;
using GServer.Common.Networking.Enums;

namespace GServer.Common.Networking.Messages.Server;

public class ListServersResponseMessage : BaseMessage, IMessage<ListServersResponseMessage>
{
    public ServerListing[] ServerListings;

    public const int SERVER_NAME_BYTES = 50;
    public const int SERVER_DESCRIPTION_BYTES = 1000;
    public const int SERVER_IP_BYTES = 15;

    public ListServersResponseMessage(ServerListing[] serverListings) : base((byte)ClientPacketIn.ListServersResponse)
    {
        ServerListings = serverListings;
    }

    public ListServersResponseMessage(MessageMemoryStream stream) : base((byte)ClientPacketIn.ListServersResponse)
    {
        List<ServerListing> serverListings = [];
        
        while (stream.Position != stream.Length)
        {
            // TODO: remove this from the packet as I don't think it's needed
            // long blockSize = stream.ReadInt64();

            ServerListing serverListing = new()
            {
                Name = stream.ReadUtf8String(SERVER_NAME_BYTES),
                Description = stream.ReadUtf8String(SERVER_DESCRIPTION_BYTES),
                Playercount = stream.ReadUInt16(),
                IpAddress = stream.ReadUtf8String(SERVER_IP_BYTES),
                Port = stream.ReadUInt16(),
                ServerTier = (ServerTier)stream.ReadByte()
            };
            
            serverListings.Add(serverListing);
        }
        
        ServerListings = serverListings.ToArray();
    }

    public byte[] Serialize()
    {
        using MessageMemoryStream stream = new();

        stream.WriteByte(PacketId);

        foreach (ServerListing listing in ServerListings)
        {
            using MessageMemoryStream listingStream = new();

            listingStream.WriteUtf8StringOfSize(SERVER_NAME_BYTES, listing.Name);
            listingStream.WriteUtf8StringOfSize(SERVER_DESCRIPTION_BYTES, listing.Description);
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
