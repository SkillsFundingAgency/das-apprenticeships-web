﻿namespace SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;

public class StandardInfo
{
    public string? CourseCode { get; set; } = null!;
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public StandardVersionInfo? StandardVersion { get; set; } = null!;
}