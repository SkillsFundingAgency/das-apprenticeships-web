using System.Web;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;

namespace SFA.DAS.Apprenticeships.Web.Models
{
    public class ViewPendingPriceChangeModel
	{
        public ViewPendingPriceChangeModel()
        {
            
        }
        public ViewPendingPriceChangeModel(string apprenticeshipHashedId, PendingPriceChange pendingPriceChange)
        {
            ApprenticeshipKey = pendingPriceChange.ApprenticeshipKey;
            ApprenticeshipHashedId = apprenticeshipHashedId;
            ApprenticeshipTrainingPrice = pendingPriceChange.PendingTrainingPrice.HasValue ? pendingPriceChange.PendingTrainingPrice!.Value : 0;
			ApprenticeshipEndPointAssessmentPrice = pendingPriceChange.PendingAssessmentPrice.Value;
			OriginalTrainingPrice = pendingPriceChange.OriginalTrainingPrice.HasValue ? pendingPriceChange.OriginalTrainingPrice.Value : 0;
			OriginalEndPointAssessmentPrice = pendingPriceChange.OriginalAssessmentPrice.Value;
			EffectiveFromDate = pendingPriceChange.EffectiveFrom;
			ReasonForChangeOfPrice = HttpUtility.HtmlDecode(pendingPriceChange.Reason);
        }

		public Guid ApprenticeshipKey { get; set; }
		public string? ApprenticeshipHashedId { get; set; }
        public decimal ApprenticeshipTrainingPrice { get; set; }
		public decimal ApprenticeshipEndPointAssessmentPrice { get; set; }
		public DateTime EffectiveFromDate { get; set; }
		public string? ReasonForChangeOfPrice { get; set; }
		public decimal ApprenticeshipTotalPrice => ApprenticeshipTrainingPrice + ApprenticeshipEndPointAssessmentPrice;
		public decimal OriginalTrainingPrice { get; set; }
		public decimal OriginalEndPointAssessmentPrice { get; set; }
		public decimal OriginalTotalPrice => OriginalTrainingPrice + OriginalEndPointAssessmentPrice;
    }
}
