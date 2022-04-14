using Radon.Exceptions;

namespace Radon.Security;

internal sealed class VoterCollection
{
    private static readonly List<VotingInvoker> Voters = new();

    internal static void Register(Type voterType)
    {
        if (!ImplementsVoterInterface(voterType))
        {
            return;
        }

        var voter = Activator.CreateInstance(voterType);
        if (voter == null)
        {
            throw new Exception("Could not create instance of voter " + voterType.FullName + "!");
        }
        
        Voters.Add(new VotingInvoker(voter));
    }

    internal static void Vote(UserBase user, object subject)
    {
        foreach (var voter in Voters.Where(voter => voter.SupportsSubject(subject) && !voter.Vote(user, subject)))
        {
            throw new VoterException(voter.VoterType, user, subject, voter.GetMessage() ?? "Unauthorized!");
        }
    }

    private static bool ImplementsVoterInterface(Type voterType)
    {
        return voterType.BaseType?.FullName != null && voterType.BaseType.FullName.StartsWith("Radon.Security.Voter");
    }
}