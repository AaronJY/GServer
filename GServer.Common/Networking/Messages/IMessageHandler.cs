using System.Net.Sockets;
using GServer.Common.Networking.Core;

namespace GServer.Common.Networking.Messages;

public interface IMessageHandler
{
    Task HandleMessageAsync(Socket clientSocket, MessageMemoryStream messageStream);
}