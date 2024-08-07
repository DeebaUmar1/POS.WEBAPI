using Microsoft.AspNetCore.Http;
using POS.Data;
using POS.Services.UserServices;
using POS.Validation;
using System.Security.Claims;
using System.Text;

namespace POS.Middlewares
{
    public class BasicAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string trainingKey = "DemoTrainingKey";

        public BasicAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserService userService)
        {
            // Check for the correct AuthKey
            if (!context.Request.Headers.TryGetValue("AuthKey", out var authKey) || authKey != trainingKey)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized: Invalid AuthKey");
                return;
            }

            // Check for Basic Authentication header
            var authHeader = context.Request.Headers["Authorization"].ToString();
            if (authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            {
                var token = authHeader.Substring("Basic ".Length).Trim();

                var credentialBytes = Convert.FromBase64String(token);

                var credentialsString = Encoding.UTF8.GetString(credentialBytes);
                Console.WriteLine("Decoded Credentials String: {0}", credentialsString);

                // Split the credentials
                var credentials = credentialsString.Split(':');
              
                // Log length and contents of credentials array
                Console.WriteLine("Credentials Length: {0}", credentials.Length);
                if (credentials.Length > 0)
                {
                    Console.WriteLine("Credentials[0]: {0}", credentials[0]);
                }
                if (credentials.Length > 1)
                {
                    Console.WriteLine("Credentials[1]: {0}", credentials[1]);
                }

                Console.WriteLine("Username: {0}, Password: {1}", credentials[0], credentials[1]);
                if (credentials.Length == 2)
                {
                    var username = credentials[0];
                    var password = credentials[1];
                   
                    // Verify credentials against the Users context
                    var user = userService.Login(username,password);
                    //Console.WriteLine(user.Result.Id);
                    if (user != null)
                    {
                        var authenticatedUser = user.Result;
                        if (authenticatedUser != null)
                        {
                            // Create claims for the authenticated user
                            Console.WriteLine(authenticatedUser.role);
                            var claims = new[]
                            {
                            new Claim(ClaimTypes.Name, username),
                            new Claim(ClaimTypes.Role, authenticatedUser.role.ToString()),// Assuming the User model has a Role property
                            new Claim(ClaimTypes.NameIdentifier, authenticatedUser.id.ToString())
                        };

                            var identity = new ClaimsIdentity(claims, "Basic");
                            context.User = new ClaimsPrincipal(identity);

                            await _next(context);
                            return;
                        }
                    }
                }
            }
             // Handle unauthorized access
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized: Invalid credentials");


            
        }
    }
}
