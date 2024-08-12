using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;
using SFA.DAS.Apprenticeships.Web.Models.Enums;

namespace SFA.DAS.Apprenticeships.Web.Models.ChangeOfStartDate;

public class ProviderChangeOfStartDateModel : BaseProviderChangeOfStartDateModel, ICacheModel, IRouteValuesProvider
{
    public DateField? ApprenticeshipActualStartDate { get; set; }
    public DateField? PlannedEndDate { get; set; }
    public InitiatedBy InitiatedBy => InitiatedBy.Provider;
    public DateTime? OriginalApprenticeshipActualStartDate { get; set; }
    public DateTime? OriginalPlannedEndDate { get; set; }
    public DateTime? EarliestStartDate { get; set; }
    public DateTime? LatestStartDate { get; set; }
    public DateTime LastFridayOfSchool { get; set; }
    public DateTime? StandardEarliestDate { get; set; }
    public DateTime? StandardLatestDate { get; set; }
    public DateTime? StandardVersionEarliestDate { get; set; }
    public DateTime? StandardVersionLatestDate { get; set; }
    public DateTime PreviousAcademicYearEndDate { get; set; }
    public DateTime PreviousAcademicYearHardCloseDate { get; set; }
    public DateTime CurrentAcademicYearStartDate { get; set; }
}

public class ProviderChangeOfStartDateModelMapper : IMapper<ProviderChangeOfStartDateModel>
{
    public ProviderChangeOfStartDateModel Map(object sourceObject)
    {
        if (sourceObject is ApprenticeshipStartDate apprenticeshipStartDate)
        {
            return FromApprenticeshipStartDate(apprenticeshipStartDate);
        }

        throw new NotImplementedException($"There is not mapping available for object of type {sourceObject.GetType().Name}");
    }

    private static ProviderChangeOfStartDateModel FromApprenticeshipStartDate(ApprenticeshipStartDate apprenticeshipStartDate)
    {
        var model = new ProviderChangeOfStartDateModel
        {
            ApprenticeshipActualStartDate = new DateField(apprenticeshipStartDate.ActualStartDate),
            OriginalApprenticeshipActualStartDate = apprenticeshipStartDate.ActualStartDate,
            OriginalPlannedEndDate = apprenticeshipStartDate.PlannedEndDate,
            ApprenticeshipKey = apprenticeshipStartDate.ApprenticeshipKey,
            EarliestStartDate = apprenticeshipStartDate.EarliestStartDate,
            LatestStartDate = apprenticeshipStartDate.LatestStartDate,
            LastFridayOfSchool = apprenticeshipStartDate.LastFridayOfSchool,
            ApprovingPartyName = apprenticeshipStartDate.EmployerName,
            StandardEarliestDate = apprenticeshipStartDate.Standard.EffectiveFrom,
            StandardLatestDate = apprenticeshipStartDate.Standard.EffectiveTo,
            StandardVersionEarliestDate = apprenticeshipStartDate.Standard.StandardVersion?.VersionEarliestStartDate,
            StandardVersionLatestDate = apprenticeshipStartDate.Standard.StandardVersion?.VersionLatestStartDate,
            PreviousAcademicYearEndDate = apprenticeshipStartDate.PreviousAcademicYear.EndDate,
            PreviousAcademicYearHardCloseDate = apprenticeshipStartDate.PreviousAcademicYear.HardCloseDate,
            CurrentAcademicYearStartDate = apprenticeshipStartDate.CurrentAcademicYear.StartDate
        };

        return model;
    }
}