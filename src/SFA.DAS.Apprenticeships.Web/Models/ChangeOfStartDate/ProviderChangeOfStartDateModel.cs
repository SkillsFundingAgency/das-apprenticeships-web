using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;
using SFA.DAS.Apprenticeships.Web.Models.Enums;

namespace SFA.DAS.Apprenticeships.Web.Models.ChangeOfStartDate;

public class ProviderChangeOfStartDateModel : BaseChangeOfStartDateModel, ICacheModel, IRouteValuesProvider
{
    public DateField? ApprenticeshipActualStartDate { get; set; }
    public long? ProviderReferenceNumber { get; set; }
    public InitiatedBy InitiatedBy => InitiatedBy.Provider;
    public DateTime? OriginalApprenticeshipActualStartDate { get; set; }
    public DateTime? EarliestStartDate { get; set; }
    public DateTime? LatestStartDate { get; set; }
    public DateTime LastFridayOfSchool { get; set; }
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
            ApprenticeshipKey = apprenticeshipStartDate.ApprenticeshipKey,
            EarliestStartDate = apprenticeshipStartDate.EarliestStartDate,
            LatestStartDate = apprenticeshipStartDate.LatestStartDate,
            LastFridayOfSchool = apprenticeshipStartDate.LastFridayOfSchool,
            ApprovingPartyName = apprenticeshipStartDate.EmployerName
        };

        model.OriginalApprenticeshipActualStartDate = model.ApprenticeshipActualStartDate.Date;

        return model;
    }
}