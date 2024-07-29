using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using System.Net;
using static POS.Middlewares.Middlewares.CustomExceptions;
using System.Text.Json;

namespace POS.Middlewares.Middlewares
{
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
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsync(new
                {
                    error = ex.Message
                }.ToString());
            }
            catch (NotFoundException ex)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                await context.Response.WriteAsync(new
                {
                    error = ex.Message
                }.ToString());
            }
            catch (UnauthorizedAccessEx ex)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync(new
                {
                    error = ex.Message
                }.ToString());
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync(new
                {
                    error = "An unexpected error occurred. Please try again later."
                }.ToString());
            }
            // Handle 403 Forbidden separately
            if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
            {
                await context.Response.WriteAsync("You do not have permission to access this resource.");
            }
          
            if (!context.User.Identity.IsAuthenticated)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("User is not authenticated.");
                return;
            }
        }
       
    }
}
