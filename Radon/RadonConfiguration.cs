namespace Radon;

/// <summary>
/// The configuration is used to configure Radon how specific elements should be used.
/// </summary>
public class RadonConfiguration
{
    /// <summary>
    /// The secret key used to encrypt the session data.
    /// </summary>
    public string Secret { get; set; } = Guid.NewGuid().ToString() + Guid.NewGuid();

    /// <summary>
    /// The amount of days the refresh tokens are stored in the <see cref="IDataStorage"/>.
    /// </summary>
    public int RefreshTokenTTL { get; set; } = 2;
    
    /// <summary>
    /// Whether a custom middleware should be enabled which handles <see cref="Exception"/>, adjusting the status code and formatting
    /// the exception to a JSON response.
    /// </summary>
    public bool EnableErrorHandler { get; set; } = true;
}