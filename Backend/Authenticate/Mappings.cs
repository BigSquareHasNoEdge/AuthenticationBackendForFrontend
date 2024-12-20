﻿using Backend.Authenticate.Login;
using Backend.Authenticate.Logout;
using Backend.Authenticate.SessionCheck;

namespace Backend.Authenticate;

static class Mappings
{
    public static void MapAuths(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth")
            ;

        group.MapSessionChecks();
        group.MapLogin();
        group.MapLogout();
    }
}