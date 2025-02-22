using GServer.Common.Networking.Messages.Client;
using GServer.Common.Networking.Messages.Server;

namespace GServer.RC.Services;

public interface IAuthService
{
    AuthResponseMessage Authenticate(AuthMessage msg);
}

public class AuthService : IAuthService
{
    public AuthService()
    {
    }

    public AuthResponseMessage Authenticate(AuthMessage msg)
    {
        throw new NotImplementedException();
    }
}