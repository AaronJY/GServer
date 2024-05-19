using System.Net;
using System.Net.Sockets;
using GServer.Common;
using GServer.Common.Networking.Enums;
using GServer.Common.Networking.Messages.Server;

internal class Program
{
    private const int SERVER_PORT = 11000;

    private static void Main(string[] args)
    {
        // var sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        var serverEP = new IPEndPoint(IPAddress.Any, SERVER_PORT);

        UdpClient udpClient = new();
        udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        udpClient.Connect(serverEP);


        // byte[] username = "helloworld123".ToUTF8String();
        // byte[] password = "password&(*$())".ToUTF8String();

        // byte[] sendBuffer = [(byte)ServerPacketIn.AUTH, (byte)username.Length, .. username, (byte)password.Length, .. password];

        Console.WriteLine("Username...");
        string username = Console.ReadLine()!;

        Console.WriteLine("Password...");
        string password = Console.ReadLine()!;

        AuthMessage authMessage = new(username, password);
        udpClient.Send(authMessage.Serialize());

        try
        {
            while (true)
            {
                byte[] bytes = udpClient.Receive(ref serverEP);

                MessageMemoryStream stream = new(bytes);

                ClientPacketIn packetIn = (ClientPacketIn)stream.ReadByte();
                switch (packetIn)
                {
                    case ClientPacketIn.AUTH_RESPONSE:
                        var authResultMessage = new AuthResponseMessage(stream);

                        Console.WriteLine("Success = " + authResultMessage.IsSuccessful);

                        Console.WriteLine("SessionToken = " + authResultMessage.SessionToken ?? "null");


                        Console.WriteLine("FailureReason = " + authResultMessage.FailureReason ?? "null");

                        break;

                    default:
                        Console.WriteLine($"Received unsupported packet.");
                        // byte[] response = [(byte)ClientPacketIn.UNKNOWN];
                        // listener.Send(response);
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
            udpClient.Close();
        }


    }
}