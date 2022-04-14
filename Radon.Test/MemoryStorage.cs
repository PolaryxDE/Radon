using Radon.Test.Entities;
using Radon.Test.Security;

namespace Radon.Test;

internal class MemoryStorage : IDataStorage
{
    public List<User> Users { get; } = new()
    {
        new User
        {
            Id = Guid.Parse("ad892b0b-efef-4280-b39a-7fa0069b3c5f"),
            Username = "Test",
            Password = "Test123",
            Roles = new List<Roles>
            {
                Roles.Moderator
            }
        }
    };

    public Task<bool> AnyUserAsync(Predicate<UserBase> predicate)
    {
        return Task.FromResult(Users.Any(user => predicate(user)));
    }

    public Task<UserBase?> GetUserForAuthAsync(HttpRequest request)
    {
        return Task.FromResult(Users.FirstOrDefault(user =>
            user.Username == request.Query["username"] && user.Password == request.Query["password"]) as UserBase);
    }

    public Task<UserBase?> GetUserAsync(Predicate<UserBase> predicate)
    {
        return Task.FromResult(Users.FirstOrDefault(user => predicate(user)) as UserBase);
    }

    public Task SaveUserAsync(UserBase user)
    {
        return Task.CompletedTask;
    }
}