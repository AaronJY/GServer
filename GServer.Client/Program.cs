using System.Net;
using System.Net.Sockets;
using GServer.Common.Game.Entities;
using GServer.Common.Networking.Core;
using GServer.Common.Networking.Enums;
using GServer.Common.Networking.Messages.Client;
using GServer.Common.Networking.Messages.Server;

namespace GServer.Client;

public static class Program
{
    private const int ServerPort = 11000;

    private static string? _sessionToken;

    private static void Main(string[] args)
    {
        IPEndPoint serverEp = new(IPAddress.Any, ServerPort);

        TcpClient tcpClient = new();
        tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        tcpClient.Connect(serverEp);
        
        try
        {
            while (_sessionToken is null)
            {
                Console.WriteLine("Username...");
                string username = Console.ReadLine()!;

                Console.WriteLine("Password...");
                string password = Console.ReadLine()!;

                AuthMessage authMessage = new(username, password);
                _ = tcpClient.Client.Send(authMessage.Serialize());
                
                while (true)
                {
                    Console.WriteLine("Listening for message...");
                    
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

                            // Request server list as soon as login has succeeded

                            // if (authResultMessage.IsSuccessful)
                            // {
                            //     Console.WriteLine("Getting server list...");
                            //     _ = tcpClient.Client.Send(new[] { (byte)ServerPacketIn.ListServers });
                            // }

                            break;

                        case ClientPacketIn.ListServersResponse:
                            ListServersResponseMessage listServersResponseMessage = new(stream);

                            Console.WriteLine("Here are the servers!");

                            foreach (ServerListing server in listServersResponseMessage.ServerListings)
                            {
                                Console.WriteLine(server.Name);
                                Console.WriteLine($"Desc: {server.Description}");
                                Console.WriteLine($"Address: {server.IpAddress}:{server.Port}");
                                Console.WriteLine($"Tier: {server.ServerTier}");
                            }
                            break;
                        
                        case ClientPacketIn.Unknown:
                            break;

                        default:
                            Console.WriteLine("Received unsupported packet.");
                            break;
                    }
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

