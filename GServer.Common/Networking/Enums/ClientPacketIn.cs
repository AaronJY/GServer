namespace GServer.Common.Networking.Enums;

public enum ClientPacketIn : byte
{
    /// <summary>
    /// Represents an auth result from the server.
    /// </summary>
    AuthResponse = 1,

    /// <summary>
    /// Contains a list of server listings.
    /// </summary>
    ListServersResponse = 2,

    Unknown = 255
}