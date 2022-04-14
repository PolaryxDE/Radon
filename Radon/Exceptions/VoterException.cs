using Radon.Security;

namespace Radon.Exceptions;

/// <summary>
/// The exception which is thrown, if a <see cref="Voter{TU,TS}"/> failed to authorize a <see cref="UserBase"/>.
/// </summary>
internal class VoterException : Exception
{
    /// <summary>
    /// The user which the voter failed to vote for.
    /// </summary>
    internal UserBase User { get; }
    
    /// <summary>
    /// The subject which was passed with the user to the voter.
    /// </summary>
    internal object Subject { get; }
    
    /// <summary>
    /// The type of the voter which failed to authorize the user.
    /// </summary>
    internal Type VoterType { get; }
    
    internal VoterException(Type voterType, UserBase user, object subject, string message) : base(message)
    {
        User = user;
        VoterType = voterType;
        Subject = subject;
    }
}