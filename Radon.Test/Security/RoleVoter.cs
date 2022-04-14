using Radon.Security;
using Radon.Test.Entities;

namespace Radon.Test.Security;

public class RoleVoter : Voter<User, Roles>
{
    public override bool Vote(User user, Roles subject)
    {
        return user.Roles.Contains(subject);
    }
}