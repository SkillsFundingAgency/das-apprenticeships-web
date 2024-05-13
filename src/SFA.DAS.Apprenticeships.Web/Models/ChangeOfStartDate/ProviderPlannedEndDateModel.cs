using SFA.DAS.Apprenticeships.Web.Extensions;

namespace SFA.DAS.Apprenticeships.Web.Models.ChangeOfStartDate;

public class ProviderPlannedEndDateModel : BaseProviderChangeOfStartDateModel
{
    public DateField? SuggestedEndDate { get; set; }
    public DateTime? OriginalPlannedEndDate { get; set; }
    public DateField? PlannedEndDate { get; set; }
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
            ApprovingPartyName = sourceObject.ApprovingPartyName,
            SuggestedEndDate = GetSuggestedEndDate(sourceObject),
            OriginalPlannedEndDate = sourceObject.OriginalPlannedEndDate
        };

        return model;
    }

    private static DateField GetSuggestedEndDate(ProviderChangeOfStartDateModel sourceObject)
    {
        var originalStartDate = sourceObject.OriginalApprenticeshipActualStartDate.ValueOrThrow(nameof(ProviderChangeOfStartDateModel.OriginalApprenticeshipActualStartDate));
        var newStartDate = sourceObject.ApprenticeshipActualStartDate!.Date.ValueOrThrow(nameof(ProviderChangeOfStartDateModel.ApprenticeshipActualStartDate));

        var dateChange = newStartDate - originalStartDate;

        var originalEndDate = sourceObject.OriginalPlannedEndDate.ValueOrThrow(nameof(ProviderChangeOfStartDateModel.OriginalPlannedEndDate));

        return new DateField(originalEndDate.AddDays(dateChange.Days));
    }
}