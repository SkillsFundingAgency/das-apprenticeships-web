namespace SFA.DAS.Apprenticeships.Domain;

public enum ChangeInitiator
{
    Provider,
	Employer
}

public static class ChangeInitiatorExtensions
{
    public static ChangeInitiator GetChangeInitiator(this string? changeInitiatorString)
    {
        if (Enum.TryParse<ChangeInitiator>(changeInitiatorString, out var initiatedBy))
        {
            return initiatedBy;
        }

        throw new ArgumentOutOfRangeException($"Could not resolve Change Initiator :{changeInitiatorString}");
    }
}
