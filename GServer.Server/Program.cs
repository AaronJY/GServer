using System.Net;
using System.Net.Sockets;
using System.Text;
using GServer.Common;
using GServer.Common.Networking.Enums;
using GServer.Common.Networking.Messages.Server;

internal class Program
{
    private const int LISTEN_PORT = 11000;

    private static void Main(string[] args)
    {

        GServer.Server.GServer server = new(
            new UdpClient(),
            new IPEndPoint(IPAddress.Any, LISTEN_PORT));

        server.Bind();

        try
        {
            IPEndPoint remoteEP = new(IPAddress.Any, 0);

            while (true)
            {
                Console.WriteLine("Waiting for message...");

                byte[] bytes = server.UdpClient.Receive(ref remoteEP);
                string ASCIIContent = Encoding.ASCII.GetString(bytes, 0, bytes.Length);

                Console.WriteLine("Received from: " + remoteEP);
                Console.WriteLine($"Length = {bytes.Length}, Content = {ASCIIContent}");

                var stream = new MessageMemoryStream(bytes);

                ServerPacketIn serverPacketIn = (ServerPacketIn)stream.ReadByte();

                switch (serverPacketIn)
                {
                    case ServerPacketIn.AUTH:
                        var msg = new AuthMessage(stream);

                        AuthResponseMessage resp;

                        if (msg.Username == "aaronyarbz" && msg.Password == "password123")
                        {
                            resp = new(true, Guid.NewGuid().ToString(), null);
                        }
                        else
                        {
                            resp = new(false, null, AuthResponseFailure.IncorrectLoginOrPassword);
                        }

                        server.UdpClient.Send(resp.Serialize(), remoteEP);

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
            Console.WriteLine("Disposing of server...");
            server.Dispose();
        }
    }
}