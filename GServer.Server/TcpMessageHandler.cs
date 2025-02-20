using System.Net.Sockets;
using GServer.Common;
using GServer.Common.Game.Entities;
using GServer.Common.Networking.Core;
using GServer.Common.Networking.Enums;
using GServer.Common.Networking.Messages;
using GServer.Common.Networking.Messages.Client;
using GServer.Common.Networking.Messages.Server;
using GServer.Server.Business.Services;

namespace GServer.Server;

public interface ITcpMessageHandler
{
    Task HandleMessageAsync(Socket clientSocket, MessageMemoryStream messageStream);
}

public class TcpMessageHandler(
    IAuthService authService
) : IMessageHandler, ITcpMessageHandler
{
    public async Task HandleMessageAsync(Socket clientSocket, MessageMemoryStream messageStream)
    {
        ServerPacketIn serverPacketIn = (ServerPacketIn)messageStream.ReadByte();

        Console.WriteLine($"Handling message {serverPacketIn} from {clientSocket.RemoteEndPoint}...");

        switch (serverPacketIn)
        {
            case ServerPacketIn.Auth:
            {
                AuthMessage msg = new(messageStream);

                bool isPasswordCorrect = authService.IsPasswordCorrect(msg.Username, msg.Password);
                AuthResponseMessage resp = isPasswordCorrect
                    ? new AuthResponseMessage(true, Guid.NewGuid().ToString(), failureReason: null)
                    : new AuthResponseMessage(false, null, AuthResponseFailure.IncorrectLoginOrPassword);
                await SendMessageAsync(resp, clientSocket);

                break;
            }

            case ServerPacketIn.ListServers:
            {
                ServerListing[] serverListings =
                [
                    new()
                    {
                        Name = "Testbed",
                        Description = "A server to hone your development skills and test new ideas.",
                        IpAddress = "255.255.255.255",
                        Port = 1337,
                        Playercount = 12,
                        ServerTier = ServerTier.Default
                    }
                ];

                ListServersResponseMessage resp = new(serverListings);
                await SendMessageAsync(resp, clientSocket);

                break;
            }

            default:
                Console.WriteLine($"Received unsupported packet.");
                break;
        }
    }
    
    private static async Task SendMessageAsync<TMessage>(
        TMessage message,
        Socket clientSocket) where TMessage : IMessage<TMessage>
    {
        byte[] buffer = message.Serialize();
        _ = await clientSocket.SendAsync(buffer);
    }
}

