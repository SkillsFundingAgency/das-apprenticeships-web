#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;

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
    public StandardInfo Standard { get; set; }
    public AcademicYearDetails CurrentAcademicYear { get; set; }
    public AcademicYearDetails PreviousAcademicYear { get; set; }
}

public class AcademicYearDetails
{
    public string AcademicYear { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime HardCloseDate { get; set; }
}