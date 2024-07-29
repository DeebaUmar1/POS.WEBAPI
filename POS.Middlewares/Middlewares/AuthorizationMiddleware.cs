using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Security.Claims;

namespace POS.Middlewares
{
    /*public class RoleBasedAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public RoleBasedAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Allow unauthenticated requests to specific paths if needed (e.g., login, register)
            var path = context.Request.Path.Value;

            if (path.StartsWith("/api/Authentication", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }
            if (path.StartsWith("/api/Authentication", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }
            // Check for authentication
            if (!context.User.Identity.IsAuthenticated)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("User is not authenticated.");
                return;
            }

            // Check for role-based authorization (simplified example)
            var userRole = context.User.FindFirst(ClaimTypes.Role)?.Value;

            // Define role-based access rules
            var allowedRoles = new Dictionary<string, string[]>
        {
                {"/api/Transaction/AddProductToSale", new [] {"Admin", "Cashier"  } },
            { "/api/Product/ViewProducts", new[] { "Admin", "Cashier" } },
            { "/api/Authentication/Register", new[] { "Admin", "Cashier" } },
            { "/api/Transaction/Sale", new[] { "Admin", "Cashier" } },
            // Add more paths and allowed roles as needed
        };

            if (allowedRoles.TryGetValue(path, out var roles))
            {
                if (!roles.Contains(userRole))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("User does not have the required role.");
                    return;
                }
            }

            await _next(context);
        }
    }*/

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
            if (path.StartsWith("/api/Authentication", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }
            if (!context.User.Identity.IsAuthenticated)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("User is not authenticated.");
                return;
            }
            
            if (path.StartsWith("/api/Transaction", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }
            var userRole = context.User.FindFirst(ClaimTypes.Role)?.Value;
            _logger.LogInformation("User Role: {Role}", userRole);

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
