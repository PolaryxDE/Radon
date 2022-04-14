namespace Radon.Models;

using System.Text.Json.Serialization;

public class AuthenticateResponse
{
    
    /// <summary>
    /// The access token for authorizing the user.
    /// </summary>
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
    
    /// <summary>
    /// The date time when the access token expires.
    /// </summary>
    [JsonPropertyName("expires_at")]
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// The refresh token is returned by the HTTP-only cookie.
    /// </summary>
    [JsonIgnore]
    public string RefreshToken { get; set; }
}