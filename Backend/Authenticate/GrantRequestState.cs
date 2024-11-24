namespace Backend.Authenticate;

record GrantRequestState(string Provider, string? ReturnUrl);