using System.Net;
using System.Net.Sockets;
using GServer.RC.UI;
using Gtk;

namespace GServer.RC;

internal static class Program
{
    private const int GameServerPort = 11000;

    private static void Main(string[] args)
    {
        Application.Init();
        
        LoginWindow loginWindow = new();
        loginWindow.Show();

        // Handle game server connection and networking on a separate thread
        Thread networkingThread = new(ConnectToGameServer);
        networkingThread.Start();
        
        Application.Run();
    }

    private static void ConnectToGameServer()
    {
        try
        {
            IPEndPoint serverEp = new(IPAddress.Any, GameServerPort);

            TcpClient tcpClient = new();
            tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            tcpClient.Connect(serverEp);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error connecting to game server: {e.Message}");
        }
    }
}