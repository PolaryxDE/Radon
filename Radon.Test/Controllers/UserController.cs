using Microsoft.AspNetCore.Mvc;
using Radon.Test.Entities;
using Radon.Test.Security;

namespace Radon.Test.Controllers;

[ApiController]
[Route("/user")]
public class UserController : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<object> GetUser(User user)
    {
        return user;
    }

    [HttpGet("admin")]
    [Authorize]
    public async Task<object> GetAdmin(User user)
    {
        user.VoteFor(Roles.Admin);

        return new
        {
            msg = "Hello Admin"
        };
    }

    [HttpGet("mod")]
    [Authorize]
    public async Task<object> GetMod(User user)
    {
        user.VoteFor(Roles.Moderator);

        return new
        {
            msg = "Hello Moderator"
        };
    }
}