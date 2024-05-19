namespace GServer.Common.Networking.Enums
{
    public enum ClientPacketIn : byte
    {
        /// <summary>
        /// Represents an auth result from the server.
        /// Format: {1(success) | 2(error)}{error msg length}{error msg}
        /// </summary>
        AUTH_RESPONSE = 1,

        UNKNOWN = 255
    }
}