using System.Net;
using System.Net.Sockets;
using GServer.Common;
using GServer.Common.Networking.Enums;
using GServer.Common.Networking.Messages.Client;
using GServer.Common.Networking.Messages.Server;

internal class Program
{
    private const int SERVER_PORT = 11000;

    private static void Main(string[] args)
    {
        IPEndPoint serverEP = new(IPAddress.Any, SERVER_PORT);

        TcpClient tcpClient = new();
        tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        tcpClient.Connect(serverEP);

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
                    case ClientPacketIn.AUTH_RESPONSE:
                        AuthResponseMessage authResultMessage = new(stream);

                        Console.WriteLine("Success = " + authResultMessage.IsSuccessful);
                        Console.WriteLine("SessionToken = " + authResultMessage.SessionToken ?? "null");
                        Console.WriteLine("FailureReason = " + authResultMessage.FailureReason ?? "null");

                        break;

                    case ClientPacketIn.LIST_SERVERS_RESPONSE:
                        break;

                    case ClientPacketIn.UNKNOWN:
                        break;

                    default:
                        Console.WriteLine($"Received unsupported packet.");
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