namespace SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public class StandardInfo
{
    public string? CourseCode { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public StandardVersionInfo? StandardVersion { get; set; }
}