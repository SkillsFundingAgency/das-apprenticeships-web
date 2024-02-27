using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;
using System.Web;

namespace SFA.DAS.Apprenticeships.Web.Models
{
	public class EmployerInitiatedEmployerViewPendingPriceChangeModel : IEmployerRouteValues
	{
		public string ApprenticeshipHashedId { get; set; }
		public Guid ApprenticeshipKey { get; set; }
		public string EmployerAccountId { get; set; }
		public string ProviderName { get; set; }
		public string BackLinkUrl { get; set; }

		public decimal OriginalTotalPrice { get; set; }
		public decimal ApprenticeshipTotalPrice { get; set; }
		public DateTime EffectiveFromDate { get; set; }
		public string? ReasonForChangeOfPrice { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

    }

	public class EmployerInitiatedEmployerViewPendingPriceChangeModelMapper : IMapper<EmployerInitiatedEmployerViewPendingPriceChangeModel>
	{
		public EmployerInitiatedEmployerViewPendingPriceChangeModel Map(object sourceObject)
		{
			if (sourceObject is GetPendingPriceChangeResponse getPendingPriceChangeResponse)
			{
				return FromGetPendingPriceChangeResponse(getPendingPriceChangeResponse);
			}

			throw new NotImplementedException($"There is not mapping available for object of type {sourceObject.GetType().Name}");
		}

		private static EmployerInitiatedEmployerViewPendingPriceChangeModel FromGetPendingPriceChangeResponse(GetPendingPriceChangeResponse getPendingPriceChangeResponse)
		{
			var model = new EmployerInitiatedEmployerViewPendingPriceChangeModel
			{
				ApprenticeshipKey = getPendingPriceChangeResponse.PendingPriceChange.ApprenticeshipKey,
				OriginalTotalPrice = getPendingPriceChangeResponse.PendingPriceChange.OriginalTotalPrice,
				ApprenticeshipTotalPrice = getPendingPriceChangeResponse.PendingPriceChange.PendingTotalPrice,
				EffectiveFromDate = getPendingPriceChangeResponse.PendingPriceChange.EffectiveFrom,
				ReasonForChangeOfPrice = HttpUtility.HtmlDecode(getPendingPriceChangeResponse.PendingPriceChange.Reason),
				ProviderName = getPendingPriceChangeResponse.ProviderName,
				FirstName = getPendingPriceChangeResponse.PendingPriceChange.FirstName,
				LastName = getPendingPriceChangeResponse.PendingPriceChange.LastName
			};

			return model;
		}
	}
}
