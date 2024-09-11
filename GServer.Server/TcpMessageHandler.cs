using System.Net.Sockets;
using GServer.Common.Networking.Core;
using GServer.Common.Networking.Enums;
using GServer.Common.Networking.Messages;
using GServer.Common.Networking.Messages.Client;
using GServer.Common.Networking.Messages.Server;

namespace GServer.Server;

public class TcpMessageHandler : IMessageHandler
{
    public async Task HandleMessageAsync(Socket clientSocket, MessageMemoryStream messageStream)
    {
        ServerPacketIn serverPacketIn = (ServerPacketIn)messageStream.ReadByte();

        Console.WriteLine($"Handling message {serverPacketIn} from {clientSocket.RemoteEndPoint}...");

        switch (serverPacketIn)
        {
            case ServerPacketIn.Auth:
                AuthMessage msg = new(messageStream);

                AuthResponseMessage resp = msg is { Username: "aaronyarbz", Password: "password123" }
                    ? new(true, Guid.NewGuid().ToString(), failureReason: null)
                    : new(false, null, AuthResponseFailure.IncorrectLoginOrPassword);

                byte[] buffer = resp.Serialize();
                _ = await clientSocket.SendAsync(buffer);

                break;

            case ServerPacketIn.ListServers:
                throw new NotImplementedException();

            default:
                Console.WriteLine($"Received unsupported packet.");
                break;
        }
    }
}