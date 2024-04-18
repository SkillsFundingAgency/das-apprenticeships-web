namespace SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;

public class StandardVersionInfo
{
    public string Version { get; set; } = null!;
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}