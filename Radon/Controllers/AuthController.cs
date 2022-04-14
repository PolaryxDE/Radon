using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Radon.Services;

namespace Radon.Controllers;

[ApiController]
[Route("/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService) => _authService = authService;

    [HttpPost("login")]
    public async Task<object> PostLogin()
    {
        var response = await _authService.AuthenticateAsync(Request, GetIPAddress());
        SetTokenCookie(response.RefreshToken);
        return Ok(response);
    }
    
    [HttpPost("refresh")]
    public async Task<object> PostRefresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var response = await _authService.RefreshTokenAsync(refreshToken, GetIPAddress());
        SetTokenCookie(response.RefreshToken);
        return Ok(response);
    }

    [HttpPost("revoke")]
    public async Task<object> PostRevoke(string? refreshToken = null)
    {
        var token = refreshToken ?? Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(token))
            return BadRequest(new { message = "Token is required" });

        await _authService.RevokeTokenAsync(token, GetIPAddress());
        return Ok(new { message = "Token revoked" });
    }
    
    private void SetTokenCookie(string token)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(7)
        };
        Response.Cookies.Append("refreshToken", token, cookieOptions);
    }
    
    private string GetIPAddress()
    {
        if (Request.Headers.ContainsKey("X-Forwarded-For"))
            return Request.Headers["X-Forwarded-For"];
        return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
    }
}