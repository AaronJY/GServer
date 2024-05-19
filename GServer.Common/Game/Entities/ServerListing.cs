namespace GServer.Common.Game.Entities;

public record ServerListing
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public ushort Playercount { get; set; }
    public required string IPAddress { get; set; }
    public ushort Port { get; set; }
    public ServerTier ServerTier { get; set; }
}
