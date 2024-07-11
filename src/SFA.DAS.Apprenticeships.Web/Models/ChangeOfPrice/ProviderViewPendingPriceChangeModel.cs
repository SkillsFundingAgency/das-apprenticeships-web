using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Responses;
using SFA.DAS.Apprenticeships.Web.Attributes;
using SFA.DAS.Apprenticeships.Web.Extensions;
using System.Web;

namespace SFA.DAS.Apprenticeships.Web.Models.ChangeOfPrice;

public class ProviderViewPendingPriceChangeModel : BasePendingPriceChangeModel, IRouteValuesProvider, IApproveRequest, ICacheModel
{
    public long? ProviderReferenceNumber { get; set; }
    public string EmployerName { get; set; } = null!;
    [RadioOption]
    public string? ApproveRequest { get; set; }
    public string? RejectReason { get; set; }
    public string? CacheKey { get; set; }
}

public class ProviderViewPendingPriceChangeModelMapper : IMapper<ProviderViewPendingPriceChangeModel>
{
    public ProviderViewPendingPriceChangeModel Map(object sourceObject)
    {
        if (sourceObject is GetPendingPriceChangeResponse getPendingPriceChangeResponse)
        {
            return FromGetPendingPriceChangeResponse(getPendingPriceChangeResponse);
        }

        throw new NotImplementedException($"There is not mapping available for object of type {sourceObject.GetType().Name}");
    }

    private static ProviderViewPendingPriceChangeModel FromGetPendingPriceChangeResponse(GetPendingPriceChangeResponse getPendingPriceChangeResponse)
    {
        var pendingPriceChange = getPendingPriceChangeResponse.PendingPriceChange;

        var model = new ProviderViewPendingPriceChangeModel
        {
            EmployerName = getPendingPriceChangeResponse.EmployerName.ValueOrSubstitute("The Employer"),
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