namespace Radon;

/// <summary>
/// If handlers which are marked by this attribute are called and are also marked with <see cref="AuthorizeAttribute"/>
/// the checking if the user is authorized or not, will be skipped.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class AllowAnonymousAttribute : Attribute
{
    
}