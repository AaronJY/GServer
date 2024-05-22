using System.Net.Sockets;

namespace GServer.Common.Networking.Messages;

public interface IMessageHandler
{
    Task HandleMessageAsync(Socket clientSocket, MessageMemoryStream messageStream);
}