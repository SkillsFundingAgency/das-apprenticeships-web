namespace SFA.DAS.Apprenticeships.Web.Models
{
    public interface IChangeOfPriceModel
    {
        public DateTime? ApprenticeshipActualStartDate { get; set; }
        public DateTime? ApprenticeshipPlannedEndDate { get; set; }
        public DateField EffectiveFromDate { get; set; }
        public string? ReasonForChangeOfPrice { get; set; }
        public string? ApprovingPartyName { get; set; }
        public InitiatedBy InitiatedBy { get; set; }
    }

    public enum InitiatedBy
	{
		Provider,
		Employer
	}
}
