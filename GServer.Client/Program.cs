using System.Net;
using System.Net.Sockets;
using GServer.Common.Networking.Core;
using GServer.Common.Networking.Enums;
using GServer.Common.Networking.Messages.Client;
using GServer.Common.Networking.Messages.Server;

namespace GServer.Client;

public class Program
{
    private const int ServerPort = 11000;

    private static void Main(string[] args)
    {
        IPEndPoint serverEp = new(IPAddress.Any, ServerPort);

        TcpClient tcpClient = new();
        tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        tcpClient.Connect(serverEp);

        Console.WriteLine("Username...");
        string username = Console.ReadLine()!;

        Console.WriteLine("Password...");
        string password = Console.ReadLine()!;

        AuthMessage authMessage = new(username, password);
        _ = tcpClient.Client.Send(authMessage.Serialize());

        try
        {
            while (true)
            {
                byte[] bytes = new byte[tcpClient.Client.ReceiveBufferSize];
                _ = tcpClient.Client.Receive(bytes);

                MessageMemoryStream stream = new(bytes);

                ClientPacketIn packetIn = (ClientPacketIn)stream.ReadByte();
                switch (packetIn)
                {
                    case ClientPacketIn.AuthResponse:
                        AuthResponseMessage authResultMessage = new(stream);

                        Console.WriteLine("Success = " + authResultMessage.IsSuccessful);
                        Console.WriteLine("SessionToken = " + authResultMessage.SessionToken);
                        Console.WriteLine("FailureReason = " + authResultMessage.FailureReason);

                        break;

                    case ClientPacketIn.ListServersResponse:
                        break;

                    case ClientPacketIn.Unknown:
                        break;

                    default:
                        Console.WriteLine("Received unsupported packet.");
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        finally
        {
            tcpClient.Close();
        }
    }
}