using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;
using System.Web;
using SFA.DAS.Apprenticeships.Web.Extensions;

namespace SFA.DAS.Apprenticeships.Web.Models.ChangeOfPrice;

public class ProviderConfirmPriceBreakdownPriceChangeModel : BasePendingPriceChangeModel, IRouteValuesProvider
{
    public long? ProviderReferenceNumber { get; set; }
}

public class ProviderConfirmPriceBreakdownPriceChangeModelMapper : IMapper<ProviderConfirmPriceBreakdownPriceChangeModel>
{
    public ProviderConfirmPriceBreakdownPriceChangeModel Map(object sourceObject)
    {
        if (sourceObject is GetPendingPriceChangeResponse getPendingPriceChangeResponse)
        {
            return FromGetPendingPriceChangeResponse(getPendingPriceChangeResponse);
        }

        throw new NotImplementedException($"There is not mapping available for object of type {sourceObject.GetType().Name}");
    }

    private static ProviderConfirmPriceBreakdownPriceChangeModel FromGetPendingPriceChangeResponse(GetPendingPriceChangeResponse getPendingPriceChangeResponse)
    {
        var pendingPriceChange = getPendingPriceChangeResponse.PendingPriceChange;

        var model = new ProviderConfirmPriceBreakdownPriceChangeModel
        {
            ApprenticeshipKey = pendingPriceChange.ApprenticeshipKey,
            ApprenticeshipTrainingPrice = pendingPriceChange.PendingTrainingPrice.ValueOrSubstitute(0),
            ApprenticeshipEndPointAssessmentPrice = pendingPriceChange.PendingAssessmentPrice.ValueOrSubstitute(0),
            OriginalTrainingPrice = pendingPriceChange.OriginalTrainingPrice.ValueOrThrow(nameof(PendingPriceChange.OriginalTrainingPrice)),
            OriginalEndPointAssessmentPrice = pendingPriceChange.OriginalAssessmentPrice.ValueOrThrow(nameof(PendingPriceChange.OriginalAssessmentPrice)),
            ApprenticeshipTotalPrice = pendingPriceChange.PendingTotalPrice,
            EffectiveFromDate = pendingPriceChange.EffectiveFrom,
            ReasonForChangeOfPrice = HttpUtility.HtmlDecode(pendingPriceChange.Reason)
        };

        return model;
    }
}