using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using static POS.Middlewares.Middlewares.CustomExceptions;

namespace POS.Middlewares.Middlewares
{
    //Handles exceptions
    public class CustomExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                if (context.Response.HasStarted)
                {
                    throw;
                }
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var response = new
                {
                    error = ex.Message
                };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            catch (NotFoundException ex)
            {
                if (context.Response.HasStarted)
                {
                    throw;
                }
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                var response = new
                {
                    error = ex.Message
                };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            catch (UnauthorizedAccessEx ex)
            {
                if (context.Response.HasStarted)
                {
                    throw;
                }
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                var response = new
                {
                    error = ex.Message
                };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            catch (Exception ex)
            {
                if (context.Response.HasStarted)
                {
                    throw;
                }
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var response = new
                {
                    error = "An unexpected error occurred. Please try again later."
                };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            //In case of forbidden exception ( User is not admin)
            if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
            {
                if (!context.Response.HasStarted)
                {
                    await context.Response.WriteAsync("You do not have permission to access this resource.");
                }
            }
            // in case user has not logged in.
           else if (!context.User.Identity.IsAuthenticated)
            {
                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("User is not authenticated.");
                }
            }
        }
    }
}
