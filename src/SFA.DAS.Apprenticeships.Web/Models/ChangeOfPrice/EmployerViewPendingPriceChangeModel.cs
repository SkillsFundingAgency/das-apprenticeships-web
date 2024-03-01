using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;
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
		var model = new EmployerViewPendingPriceChangeModel
		{
			ApprenticeshipKey = getPendingPriceChangeResponse.PendingPriceChange.ApprenticeshipKey,
			ApprenticeshipTrainingPrice = getPendingPriceChangeResponse.PendingPriceChange.PendingTrainingPrice.HasValue ? getPendingPriceChangeResponse.PendingPriceChange.PendingTrainingPrice!.Value : 0,
			ApprenticeshipEndPointAssessmentPrice = getPendingPriceChangeResponse.PendingPriceChange.PendingAssessmentPrice.Value,
			OriginalTrainingPrice = getPendingPriceChangeResponse.PendingPriceChange.OriginalTrainingPrice.HasValue ? getPendingPriceChangeResponse.PendingPriceChange.OriginalTrainingPrice.Value : 0,
			OriginalEndPointAssessmentPrice = getPendingPriceChangeResponse.PendingPriceChange.OriginalAssessmentPrice.Value,
			EffectiveFromDate = getPendingPriceChangeResponse.PendingPriceChange.EffectiveFrom,
			ReasonForChangeOfPrice = HttpUtility.HtmlDecode(getPendingPriceChangeResponse.PendingPriceChange.Reason),
			ProviderName = getPendingPriceChangeResponse.ProviderName
		};

		return model;
	}
}