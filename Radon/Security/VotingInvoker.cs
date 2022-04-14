using System.Reflection;

namespace Radon.Security;

internal class VotingInvoker
{
    internal Type VoterType => _voterInstance.GetType();
    
    private readonly object _voterInstance;
    private readonly MethodInfo _voteMethod, _supportsMethod, _getMessageMethod;

    internal VotingInvoker(object voterInstance)
    {
        _voterInstance = voterInstance;
        _supportsMethod = _voterInstance.GetType().GetMethod("SupportsSubject") ?? throw new ArgumentException($"The voter {voterInstance.GetType().Name} does not implement the SupportsSubject method.");
        _voteMethod = _voterInstance.GetType().GetMethod("Vote") ?? throw new ArgumentException($"The voter {voterInstance.GetType().Name} does not implement the Vote method.");
        _getMessageMethod = _voterInstance.GetType().GetMethod("GetMessage") ?? throw new ArgumentException($"The voter {voterInstance.GetType().Name} does not implement the GetMessage method.");
    }

    internal bool SupportsSubject(object subject)
    {
        return _supportsMethod.Invoke(_voterInstance, new[] {subject}) as bool? ?? false;
    }
    
    internal bool Vote(UserBase user, object subject)
    {
        return _voteMethod.Invoke(_voterInstance, new[] {user, subject}) as bool? ?? false;
    }

    internal string? GetMessage()
    {
        return _getMessageMethod.Invoke(_voterInstance, Array.Empty<object>()) as string;
    }
}