using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;
using SFA.DAS.Apprenticeships.Web.Models.Enums;

namespace SFA.DAS.Apprenticeships.Web.Models.ChangeOfStartDate;

public class ProviderChangeOfStartDateModel : BaseChangeOfStartDateModel, ICacheModel, IRouteValuesProvider
{
    public DateTime? ApprenticeshipActualStartDate => new DateTime(StartYear, StartMonth, StartDay);
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

    private static ProviderChangeOfStartDateModel FromApprenticeshipStartDate(ApprenticeshipStartDate apprenticeshipStartDate)
    {
        var model = new ProviderChangeOfStartDateModel
        {
            StartYear = apprenticeshipStartDate.ActualStartDate.GetValueOrDefault().Year,
            StartMonth = apprenticeshipStartDate.ActualStartDate.GetValueOrDefault().Month,
            StartDay = apprenticeshipStartDate.ActualStartDate.GetValueOrDefault().Day,
            ApprenticeshipKey = apprenticeshipStartDate.ApprenticeshipKey
        };

        model.OriginalApprenticeshipActualStartDate = model.ApprenticeshipActualStartDate;

        return model;
    }
}