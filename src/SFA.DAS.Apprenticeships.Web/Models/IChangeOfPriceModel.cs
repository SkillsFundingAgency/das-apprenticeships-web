namespace SFA.DAS.Apprenticeships.Web.Models
{
    public abstract class BaseChangeOfPriceModel
    {
        public string? CacheKey { get; set; }
        public Guid ApprenticeshipKey { get; set; }
        public string? ApprenticeshipHashedId { get; set; }
        public int FundingBandMaximum { get; set; }
        public DateTime? EarliestEffectiveDate { get; set; }
        public DateTime? ApprenticeshipActualStartDate { get; set; }
        public DateTime? ApprenticeshipPlannedEndDate { get; set; }
        public DateField EffectiveFromDate { get; set; } = new DateField();
        public string? ReasonForChangeOfPrice { get; set; }
        public string? ApprovingPartyName { get; set; }
    }

    public interface IChangeOfPriceModel
    {
        public string? ApprenticeshipHashedId { get; set; }
        public int FundingBandMaximum { get; set; }
        public DateTime? ApprenticeshipActualStartDate { get; set; }
        public DateTime? ApprenticeshipPlannedEndDate { get; set; }
        public DateField EffectiveFromDate { get; set; }
        public string? ReasonForChangeOfPrice { get; set; }
        public string? ApprovingPartyName { get; set; }
        public InitiatedBy InitiatedBy { get; }
    }

    public enum InitiatedBy
	{
		Provider,
		Employer
	}
}
