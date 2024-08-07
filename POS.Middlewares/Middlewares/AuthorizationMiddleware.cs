using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Security.Claims;

namespace POS.Middlewares
{
    //This middleware works for basic authentication.
    //To authorize a user based on roles.
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuthorizationMiddleware> _logger;
        private readonly string _requiredRole;

        public AuthorizationMiddleware(RequestDelegate next, ILogger<AuthorizationMiddleware> logger, string requiredRole)
        {
            _next = next;
            _logger = logger;
            _requiredRole = requiredRole;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;
            //This path is for every kind of user. (anonymous)
            if (path.StartsWith("/api/Authentication", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }
            //To see if a user has logged in or not.
           if (!context.User.Identity.IsAuthenticated)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("User is not authenticated.");
                return;
            }
            // This path is for all the roles of authenticated user.
            // Generally, admin is the person who has access to every functionality
            // That's why admin and cashier both has access to this api
            if (path.StartsWith("/api/Transaction", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }
            // Get user role from claims
            var userRole = context.User.FindFirst(ClaimTypes.Role)?.Value;
            _logger.LogInformation("User Role: {Role}", userRole);

            // in case user is not Admin
            if (userRole != _requiredRole)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("User does not have the required role.");
                return;
            }

            await _next(context);
        }
    }

}
