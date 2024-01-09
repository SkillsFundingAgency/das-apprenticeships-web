using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;

namespace SFA.DAS.Apprenticeships.Web.Models
{
    public class ViewPendingPriceChangeModel
	{
		public ViewPendingPriceChangeModel(Guid apprenticeshipKey, string apprenticeshipHashedId, PendingPriceChange pendingPriceChange)
        {
            ApprenticeshipKey = apprenticeshipKey;
            ApprenticeshipHashedId = apprenticeshipHashedId;
            ApprenticeshipTrainingPrice = pendingPriceChange.PendingTrainingPrice.Value;
			ApprenticeshipEndPointAssessmentPrice = pendingPriceChange.PendingAssessmentPrice.Value;
			OriginalTrainingPrice = pendingPriceChange.OriginalTrainingPrice.Value;
			OriginalEndPointAssessmentPrice = pendingPriceChange.OriginalAssessmentPrice.Value;
			EffectiveFromDate = pendingPriceChange.EffectiveFrom;
			ReasonForChangeOfPrice = pendingPriceChange.Reason;
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
    }
}
