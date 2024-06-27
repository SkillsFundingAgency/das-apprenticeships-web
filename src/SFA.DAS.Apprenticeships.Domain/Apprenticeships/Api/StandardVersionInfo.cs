#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;

public class StandardVersionInfo
{
    public string Version { get; set; }
    public DateTime? VersionEarliestStartDate { get; set; }
    public DateTime? VersionLatestStartDate { get; set; }
}