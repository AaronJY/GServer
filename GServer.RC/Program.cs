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
            string gameServerIp = IPAddress.Any.ToString();
            Static.TcpGameClient.ConnectAsync(gameServerIp, GameServerPort).Wait();
            Console.WriteLine($"Successfully connected to game server @ {gameServerIp}:{GameServerPort}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error connecting to game server: {e.Message}");
        }
    }
}