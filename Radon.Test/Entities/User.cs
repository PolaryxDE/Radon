using Radon.Test.Security;

namespace Radon.Test.Entities;

public class User : UserBase
{
    public string Username { get; set; }
    
    public string Password { get; set; }

    public List<Roles> Roles { get; set; } = new();
}