using GServer.Common.Game.Entities;

namespace GServer.Server.Services;

public class ServerListService
{
    public ServerListService()
    {
    }

    public IEnumerable<ServerListing> List()
    {
        return [
            new ServerListing
            {
                Name = "Smallville",
                Description = "A tiny development server!",
                IPAddress = "localhost",
                Port = 11001,
                Playercount = 1,
            }
        ];
    }
}