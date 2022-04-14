namespace Radon.Security;

/// <summary>
/// The voter is a mechanism which allows special authorization rules for <see cref="UserBase"/> like a role system.
/// The voter itself takes care of the "voting" process, which is the process of checking if the <see cref="UserBase"/>
/// fits for the voter's rules.
/// </summary>
/// <typeparam name="TU">The type which inherits the <see cref="UserBase"/>.</typeparam>
/// <typeparam name="TS">The type of the subject used for this voter.</typeparam>
public abstract class Voter<TU, TS> where TU : UserBase
{
    /// <summary>
    /// Executes the voting process with the given user and the given subject.
    /// </summary>
    /// <param name="user">The user to be authorized.</param>
    /// <param name="subject">The subject for the voting process.</param>
    /// <returns>True, if the user is authorized, false if the voting failed.</returns>
    public abstract bool Vote(TU user, TS subject);
    
    /// <summary>
    /// Checks if the given subject type is supported by this voter.
    /// </summary>
    /// <param name="subject">The subject to be used for the voting.</param>
    /// <returns>True, if the voter supports the subject.</returns>
    public virtual bool SupportsSubject(object subject)
    {
        return subject is TS;
    }

    /// <summary>
    /// Returns a special message which will be used for the failure message of the voting process.
    /// </summary>
    public virtual string? GetMessage() => null;
}