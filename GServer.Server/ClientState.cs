using System.Net.Sockets;

namespace GServer.Server;

/// <summary>
/// Holds information related to a connected client.
/// </summary>
/// <param name="client"></param>
public class ClientState(
    TcpClient client)
{
    public TcpClient Client { get; } = client;
    /// <summary>
    /// The ID of the associated player.
    /// </summary>
    public Guid PlayerId { get; set; }
    /// <summary>
    /// The username of the associated player.
    /// </summary>
    public string Username { get; set; }
    /// <summary>
    /// The timestamp (UTC) of the last received packet from the client.
    /// </summary>
    public DateTime LastHeartbeat { get; set; }
}