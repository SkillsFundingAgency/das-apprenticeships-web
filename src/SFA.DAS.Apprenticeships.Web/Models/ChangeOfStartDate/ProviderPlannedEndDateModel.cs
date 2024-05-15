using SFA.DAS.Apprenticeships.Web.Extensions;

namespace SFA.DAS.Apprenticeships.Web.Models.ChangeOfStartDate;

public class ProviderPlannedEndDateModel : ProviderChangeOfStartDateModel
{
    public DateTime? SuggestedEndDate => GetSuggestedEndDate();
    public DateTime? MiniumEndDate => ApprenticeshipActualStartDate?.Date?.AddDays(365);
    public bool? UseSuggestedDate { get; set; }

    private DateTime? GetSuggestedEndDate()
    {
        if (!OriginalApprenticeshipActualStartDate.HasValue || !OriginalPlannedEndDate.HasValue || ApprenticeshipActualStartDate?.Date.HasValue != true)
            return null;

        var dateChange = ApprenticeshipActualStartDate.Date - OriginalApprenticeshipActualStartDate;

        return OriginalPlannedEndDate.Value.AddDays(dateChange!.Value.Days);
    }
}

public class ProviderPlannedEndDateModelMapper : IMapper<ProviderPlannedEndDateModel>
{
    public ProviderPlannedEndDateModel Map(object sourceObject)
    {
        if (sourceObject is ProviderChangeOfStartDateModel getPendingStartDateChangeResponse)
        {
            return FromProviderChangeOfStartDateModel(getPendingStartDateChangeResponse);
        }

        throw new NotImplementedException($"There is not mapping available for object of type {sourceObject.GetType().Name}");
    }

    private static ProviderPlannedEndDateModel FromProviderChangeOfStartDateModel(ProviderChangeOfStartDateModel sourceObject)
    {
        var model = new ProviderPlannedEndDateModel
        {
            ApprenticeshipKey = sourceObject.ApprenticeshipKey,
            ProviderReferenceNumber = sourceObject.ProviderReferenceNumber,
            CacheKey = sourceObject.CacheKey,
            ApprenticeshipHashedId = sourceObject.ApprenticeshipHashedId,
            ApprenticeshipActualStartDate = sourceObject.ApprenticeshipActualStartDate,
            PlannedEndDate = sourceObject.PlannedEndDate,
            OriginalApprenticeshipActualStartDate = sourceObject.OriginalApprenticeshipActualStartDate,
            OriginalPlannedEndDate = sourceObject.OriginalPlannedEndDate
        };

        return model;
    }
}