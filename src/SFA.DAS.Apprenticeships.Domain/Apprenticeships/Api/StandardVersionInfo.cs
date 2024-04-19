namespace SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;

public class StandardVersionInfo
{
    public string Version { get; set; } = null!;
    public DateTime? VersionEarliestStartDate { get; set; }
    public DateTime? VersionLatestStartDate { get; set; }
}