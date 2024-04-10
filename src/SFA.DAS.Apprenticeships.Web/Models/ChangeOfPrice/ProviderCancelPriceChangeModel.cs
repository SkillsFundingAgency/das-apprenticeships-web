using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;
using SFA.DAS.Apprenticeships.Web.Extensions;
using System.Web;

namespace SFA.DAS.Apprenticeships.Web.Models.ChangeOfPrice;

public class ProviderCancelPriceChangeModel : BasePendingPriceChangeModel, IRouteValuesProvider
{
    public long? ProviderReferenceNumber { get; set; }
}

public class ProviderCancelPriceChangeModelMapper : IMapper<ProviderCancelPriceChangeModel>
{
    public ProviderCancelPriceChangeModel Map(object sourceObject)
    {
        if (sourceObject is GetPendingPriceChangeResponse getPendingPriceChangeResponse)
        {
            return FromGetPendingPriceChangeResponse(getPendingPriceChangeResponse);
        }

        throw new NotImplementedException($"There is not mapping available for object of type {sourceObject.GetType().Name}");
    }

    private static ProviderCancelPriceChangeModel FromGetPendingPriceChangeResponse(GetPendingPriceChangeResponse getPendingPriceChangeResponse)
    {
        var pendingPriceChange = getPendingPriceChangeResponse.PendingPriceChange;
        var model = new ProviderCancelPriceChangeModel
        {
            ApprenticeshipKey = pendingPriceChange.ApprenticeshipKey,
            ApprenticeshipTrainingPrice = pendingPriceChange.PendingTrainingPrice,
            ApprenticeshipEndPointAssessmentPrice = pendingPriceChange.PendingAssessmentPrice,
            OriginalTrainingPrice = pendingPriceChange.OriginalTrainingPrice.ValueOrThrow(nameof(PendingPriceChange.OriginalTrainingPrice)),
            OriginalEndPointAssessmentPrice = pendingPriceChange.OriginalAssessmentPrice.ValueOrThrow(nameof(PendingPriceChange.OriginalAssessmentPrice)),
            EffectiveFromDate = pendingPriceChange.EffectiveFrom,
            ReasonForChangeOfPrice = HttpUtility.HtmlDecode(pendingPriceChange.Reason),
        };

        model.ApprenticeshipTotalPrice = model.ApprenticeshipTrainingPrice + model.ApprenticeshipEndPointAssessmentPrice;

        return model;
    }
}

