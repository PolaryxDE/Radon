using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Radon.Exceptions;

namespace Radon.Middlewares;

internal class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            response.StatusCode = error switch
            {
                VoterException => (int) HttpStatusCode.Unauthorized,
                RadonException => (int) HttpStatusCode.BadRequest,
                KeyNotFoundException => (int) HttpStatusCode.NotFound,
                _ => (int) HttpStatusCode.InternalServerError
            };

            var result = JsonSerializer.Serialize(new
            {
                message = error.Message,
                stacktrace = Debugger.IsAttached ? error.StackTrace : null
            });
            await response.WriteAsync(result);
        }
    }
}