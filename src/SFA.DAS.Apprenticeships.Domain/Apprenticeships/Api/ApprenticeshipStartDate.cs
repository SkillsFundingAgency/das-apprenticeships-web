﻿namespace SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;

public class ApprenticeshipStartDate
{
    public Guid ApprenticeshipKey { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? PlannedEndDate { get; set; }
    public string? EmployerName { get; set; }
    public string? ProviderName { get; set; }
    public DateTime? EarliestStartDate { get; set; }
    public DateTime? LatestStartDate { get; set; }
    public DateTime LastFridayOfSchool { get; set; }
}