using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using POS.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

public class BearerTokenMiddleware
{
    private readonly RequestDelegate _next;
    private readonly TokenServices _tokenService;
    
    public BearerTokenMiddleware(RequestDelegate next, TokenServices tokenService)
    {
        _next = next;
        _tokenService = tokenService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (token != null)
        {
            try
            {
                var principal = _tokenService.GetPrincipalFromToken(token);
                if (principal != null)
                {

               //     context.User = principal;
               
                    context.User = new ClaimsPrincipal(principal);
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
            }
            catch (Exception)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }
        }

        await _next(context);
    }
}
