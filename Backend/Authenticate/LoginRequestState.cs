namespace Backend.Authenticate;

record LoginRequestState(string Provider, string? ReturnUrl);