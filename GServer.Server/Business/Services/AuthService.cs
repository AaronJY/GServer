namespace GServer.Server.Business.Services;

public interface IAuthService
{
    /// <summary>
    /// Checks whether a given email and password combination match.
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    bool IsPasswordCorrect(string email, string password);
}

public class AuthService : IAuthService
{
    public bool IsPasswordCorrect(string username, string password)
    {
        // TODO: Check DB
        return username == "aaronyarbz" && password == "password123";
    }
}