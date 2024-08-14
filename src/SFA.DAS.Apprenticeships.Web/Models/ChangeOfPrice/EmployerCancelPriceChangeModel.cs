using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Responses;
using SFA.DAS.Apprenticeships.Web.Attributes;
using System.Web;

namespace SFA.DAS.Apprenticeships.Web.Models.ChangeOfPrice;

public class EmployerCancelPriceChangeModel : IRouteValuesEmployer, ICancelRequest, ICacheModel
{
	public string? ApprenticeshipHashedId { get; set; } = string.Empty;
	public Guid ApprenticeshipKey { get; set; }
	public string EmployerAccountId { get; set; } = string.Empty;
	public string ProviderName { get; set; } = string.Empty;
	public string BackLinkUrl { get; set; } = string.Empty;
	public decimal OriginalTotalPrice { get; set; }
	public decimal ApprenticeshipTotalPrice { get; set; }
	public DateTime EffectiveFromDate { get; set; }
	public string? ReasonForChangeOfPrice { get; set; }
	public string? FirstName { get; set; }
	public string? LastName { get; set; }
    [RadioOption]
    public string? CancelRequest { get; set; }
	public string? CacheKey { get; set; }
}

public class EmployerCancelPriceChangeModelMapper : IMapper<EmployerCancelPriceChangeModel>
{
	public EmployerCancelPriceChangeModel Map(object sourceObject)
	{
		if (sourceObject is GetPendingPriceChangeResponse getPendingPriceChangeResponse)
		{
			return FromGetPendingPriceChangeResponse(getPendingPriceChangeResponse);
		}

		throw new NotImplementedException($"There is not mapping available for object of type {sourceObject.GetType().Name}");
	}

	private static EmployerCancelPriceChangeModel FromGetPendingPriceChangeResponse(GetPendingPriceChangeResponse getPendingPriceChangeResponse)
	{
		var model = new EmployerCancelPriceChangeModel
        {
			ApprenticeshipKey = getPendingPriceChangeResponse.PendingPriceChange.ApprenticeshipKey,
			OriginalTotalPrice = getPendingPriceChangeResponse.PendingPriceChange.OriginalTotalPrice,
			ApprenticeshipTotalPrice = getPendingPriceChangeResponse.PendingPriceChange.PendingTotalPrice,
			EffectiveFromDate = getPendingPriceChangeResponse.PendingPriceChange.EffectiveFrom,
			ReasonForChangeOfPrice = HttpUtility.HtmlDecode(getPendingPriceChangeResponse.PendingPriceChange.Reason),
			ProviderName = getPendingPriceChangeResponse.ProviderName!,
			FirstName = getPendingPriceChangeResponse.PendingPriceChange.FirstName,
			LastName = getPendingPriceChangeResponse.PendingPriceChange.LastName
		};

		return model;
	}
}
