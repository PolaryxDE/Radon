using Microsoft.AspNetCore.Http;
using Radon.Exceptions;
using Radon.Models;

namespace Radon.Services;

public class AuthService
{
    private readonly IDataStorage _storage;
    private readonly JwtService _jwtService;
    private readonly RadonConfiguration _configuration;

    public AuthService(IDataStorage storage, JwtService jwtService, RadonConfiguration configuration)
    {
        _storage = storage;
        _jwtService = jwtService;
        _configuration = configuration;
    }

    public Task<UserBase?> GetUserByIdAsync(Guid id)
    {
        return _storage.GetUserAsync(user => user.Id == id);
    }

    public async Task<AuthenticateResponse> AuthenticateAsync(HttpRequest request, string ipAddress)
    {
        var user = await _storage.GetUserForAuthAsync(request);
        if (user == null)
            throw new RadonException("User not found");

        var jwtToken = _jwtService.GenerateJwtToken(user, out var exp);
        var refreshToken = await _jwtService.GenerateRefreshTokenAsync(ipAddress);
        user.RefreshTokens.Add(refreshToken);

        RemoveOldRefreshTokens(user);

        await _storage.SaveUserAsync(user);

        return new AuthenticateResponse
        {
            AccessToken = jwtToken,
            ExpiresAt = exp,
            RefreshToken = refreshToken.Token
        };
    }

    public async Task<AuthenticateResponse> RefreshTokenAsync(string token, string ipAddress)
    {
        var user = await GetUserByRefreshTokenAsync(token);
        var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

        if (refreshToken.IsRevoked)
        {
            RevokeDescendantRefreshTokens(refreshToken, user, ipAddress, $"Attempted reuse of revoked ancestor token: {token}");

            await _storage.SaveUserAsync(user);
        }

        if (!refreshToken.IsActive)
            throw new RadonException("Invalid token");

        var newRefreshToken = await RotateRefreshTokenAsync(refreshToken, ipAddress);
        user.RefreshTokens.Add(newRefreshToken);

        RemoveOldRefreshTokens(user);

        await _storage.SaveUserAsync(user);

        var jwtToken = _jwtService.GenerateJwtToken(user, out var exp);
        return new AuthenticateResponse
        {
            AccessToken = jwtToken,
            ExpiresAt = exp,
            RefreshToken = newRefreshToken.Token
        };
    }

    public async Task RevokeTokenAsync(string token, string ipAddress)
    {
        var user = await GetUserByRefreshTokenAsync(token);
        var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

        if (!refreshToken.IsActive)
            throw new RadonException("Invalid token");

        RevokeRefreshToken(refreshToken, ipAddress, "Revoked without replacement");

        await _storage.SaveUserAsync(user);
    }
    
    private async Task<UserBase> GetUserByRefreshTokenAsync(string token)
    {
        var user = await _storage.GetUserAsync(u => u.RefreshTokens.Any(t => t.Token == token));

        if (user == null)
            throw new RadonException("Invalid token");

        return user;
    }

    private async Task<RefreshToken> RotateRefreshTokenAsync(RefreshToken refreshToken, string ipAddress)
    {
        var newRefreshToken = await _jwtService.GenerateRefreshTokenAsync(ipAddress);
        RevokeRefreshToken(refreshToken, ipAddress, "Replaced by new token", newRefreshToken.Token);
        return newRefreshToken;
    }

    private void RemoveOldRefreshTokens(UserBase user)
    {
        user.RefreshTokens.RemoveAll(x => !x.IsActive && 
                                          x.Created.AddDays(_configuration.RefreshTokenTTL) <= DateTime.UtcNow);
    }

    private void RevokeDescendantRefreshTokens(RefreshToken refreshToken, UserBase user, string ipAddress, string reason)
    {
        if(!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
        {
            var childToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken.ReplacedByToken);
            if (childToken != null)
            {
                if (childToken.IsActive)
                {
                    RevokeRefreshToken(childToken, ipAddress, reason);
                }
                else
                {
                    RevokeDescendantRefreshTokens(childToken, user, ipAddress, reason);
                }
            }
        }
    }

    private void RevokeRefreshToken(RefreshToken token, string ipAddress, string reason = null, string replacedByToken = null)
    {
        token.Revoked = DateTime.UtcNow;
        token.RevokedByIp = ipAddress;
        token.ReasonRevoked = reason;
        token.ReplacedByToken = replacedByToken;
    }
}