namespace GServer.Common.Networking.Enums;

public enum ClientPacketIn : byte
{
    /// <summary>
    /// Represents an auth result from the server.
    /// </summary>
    AUTH_RESPONSE = 1,

    /// <summary>
    /// Contains a list of server listings.
    /// </summary>
    LIST_SERVERS_RESPONSE = 2,

    UNKNOWN = 255
}