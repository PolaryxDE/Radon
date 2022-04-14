using Microsoft.AspNetCore.Http;

namespace Radon;

/// <summary>
/// The data storage is used to define the data storage for the application. It must be implemented and given to Radon
/// so that it can manage its data without interfering with the application's real data storage.
/// </summary>
public interface IDataStorage
{
    /// <summary>
    /// Checks if any <see cref="UserBase"/> exists in the data storage which fits to the given callback.
    /// </summary>
    /// <param name="predicate">The callback which defines the filter.</param>
    /// <returns>True, if at least one <see cref="UserBase"/> fits to the criteria.</returns>
    Task<bool> AnyUserAsync(Predicate<UserBase> predicate);

    /// <summary>
    /// Returns a <see cref="UserBase"/> by the given <see cref="HttpRequest"/>.
    /// If no user was found, null is returned.
    /// </summary>
    Task<UserBase?> GetUserForAuthAsync(HttpRequest request);

    /// <summary>
    /// Returns the user which fits to the given predicate, or null if none is found.
    /// </summary>
    Task<UserBase?> GetUserAsync(Predicate<UserBase> predicate);

    /// <summary>
    /// Saves the given user into the storage.
    /// </summary>
    /// <param name="user">The user to be saved.</param>
    Task SaveUserAsync(UserBase user);
}