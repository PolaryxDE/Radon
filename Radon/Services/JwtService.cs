using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Radon.Services;

public class JwtService
{
    private readonly IDataStorage _storage;
    private readonly RadonConfiguration _configuration;

    public JwtService(IDataStorage storage, RadonConfiguration configuration)
    {
        _storage = storage;
        _configuration = configuration;
    }

    public string GenerateJwtToken(UserBase user, out DateTime exp)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
            Expires = exp = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public Guid? ValidateJwtToken(string token)
    {
        if (token == null)
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration.Secret);
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = Guid.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);
            return userId;
        }
        catch
        {
            return null;
        }
    }

    public async Task<RefreshToken> GenerateRefreshTokenAsync(string ipAddress)
    {
        var refreshToken = new RefreshToken
        {
            Token = await GetUniqueToken(),
            Expires = DateTime.UtcNow.AddDays(7),
            Created = DateTime.UtcNow,
            CreatedByIp = ipAddress
        };

        return refreshToken;

        async Task<string> GetUniqueToken()
        {
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var tokenIsUnique = !await _storage.AnyUserAsync(u => u.RefreshTokens.Any(t => t.Token == token));
            return !tokenIsUnique ? await GetUniqueToken() : token;
        }
    }
}