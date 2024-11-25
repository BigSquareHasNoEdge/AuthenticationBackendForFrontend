namespace Backend.Contract.Authenticate.Routes;
public record GetResponse(string LoginRoute, string LogoutRoute, string SessionCheckRoute, string[] Providers);


