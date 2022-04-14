using Microsoft.AspNetCore.Http;
using Radon.Services;

namespace Radon.Middlewares;

internal class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly AuthService _authService;
    private readonly JwtService _jwtService;

    public JwtMiddleware(RequestDelegate next, AuthService authService, JwtService jwtService)
    {
        _next = next;
        _authService = authService;
        _jwtService = jwtService;
    }

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        if (!string.IsNullOrEmpty(token))
        {
            var userId = _jwtService.ValidateJwtToken(token);
            if (userId != null)
            {
                context.Items["User"] = await _authService.GetUserByIdAsync(userId.Value);
            }
        }

        await _next(context);
    }
}