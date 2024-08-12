using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Responses;
using SFA.DAS.Apprenticeships.Web.Extensions;
using System.Web;

namespace SFA.DAS.Apprenticeships.Web.Models.ChangeOfPrice;

public class EmployerViewPendingPriceChangeModel : BasePendingPriceChangeModel, IRouteValuesEmployer
{
    public string EmployerAccountId { get; set; }
    public string ProviderName { get; set; }
    public string BackLinkUrl { get; set; }
}

public class EmployerViewPendingPriceChangeModelMapper : IMapper<EmployerViewPendingPriceChangeModel>
{
	public EmployerViewPendingPriceChangeModel Map(object sourceObject)
	{
		if (sourceObject is GetPendingPriceChangeResponse getPendingPriceChangeResponse)
		{
			return FromGetPendingPriceChangeResponse(getPendingPriceChangeResponse);
		}

		throw new NotImplementedException($"There is not mapping available for object of type {sourceObject.GetType().Name}");
	}

	private static EmployerViewPendingPriceChangeModel FromGetPendingPriceChangeResponse(GetPendingPriceChangeResponse getPendingPriceChangeResponse)
	{
		var pendingPriceChange = getPendingPriceChangeResponse.PendingPriceChange;

		var model = new EmployerViewPendingPriceChangeModel
		{
			ApprenticeshipKey = pendingPriceChange.ApprenticeshipKey,
			ApprenticeshipTrainingPrice = pendingPriceChange.PendingTrainingPrice.ValueOrSubstitute(0),
			ApprenticeshipEndPointAssessmentPrice = pendingPriceChange.PendingAssessmentPrice.ValueOrSubstitute(0),
			OriginalTrainingPrice = pendingPriceChange.OriginalTrainingPrice.ValueOrSubstitute(0),
			OriginalEndPointAssessmentPrice = pendingPriceChange.OriginalAssessmentPrice.ValueOrSubstitute(0),
			EffectiveFromDate = pendingPriceChange.EffectiveFrom,
			ReasonForChangeOfPrice = HttpUtility.HtmlDecode(pendingPriceChange.Reason),
			ProviderName = getPendingPriceChangeResponse.ProviderName.ValueOrSubstitute("The Provider")
        };

		model.ApprenticeshipTotalPrice = model.ApprenticeshipTrainingPrice + model.ApprenticeshipEndPointAssessmentPrice;

		return model;
	}
}