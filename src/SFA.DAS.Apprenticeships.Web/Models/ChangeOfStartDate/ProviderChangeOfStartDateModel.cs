using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;
using SFA.DAS.Apprenticeships.Web.Models.Enums;

namespace SFA.DAS.Apprenticeships.Web.Models.ChangeOfStartDate;

public class ProviderChangeOfStartDateModel : BaseChangeOfStartDateModel, ICacheModel, IRouteValuesProvider
{
    public DateTime? ApprenticeshipActualStartDate { get; set; }
    public long? ProviderReferenceNumber { get; set; }
    public InitiatedBy InitiatedBy => InitiatedBy.Provider;
    public DateTime? OriginalApprenticeshipActualStartDate { get; set; }
    public int StartDay { get; set; }
    public int StartMonth { get; set; }
    public int StartYear { get; set; }
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

    private static ProviderChangeOfStartDateModel FromApprenticeshipStartDate(ApprenticeshipStartDate apprenticeshipPrice)
    {
        var model = new ProviderChangeOfStartDateModel
        {
            ApprenticeshipActualStartDate = apprenticeshipPrice.ActualStartDate,
            ApprenticeshipKey = apprenticeshipPrice.ApprenticeshipKey
        };

        model.OriginalApprenticeshipActualStartDate = model.ApprenticeshipActualStartDate;

        return model;
    }
}