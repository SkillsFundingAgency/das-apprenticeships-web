namespace SFA.DAS.Apprenticeships.Web.Models.ChangeOfPrice;

public abstract class BasePendingPriceChangeModel
{
    public Guid ApprenticeshipKey { get; set; }
    public string? ApprenticeshipHashedId { get; set; }
    public decimal? ApprenticeshipTrainingPrice { get; set; }
    public decimal? ApprenticeshipEndPointAssessmentPrice { get; set; }
    public DateTime EffectiveFromDate { get; set; }
    public string? ReasonForChangeOfPrice { get; set; }
    public decimal? ApprenticeshipTotalPrice { get; set; }
    public decimal OriginalTrainingPrice { get; set; }
    public decimal OriginalEndPointAssessmentPrice { get; set; }
    public decimal OriginalTotalPrice => OriginalTrainingPrice + OriginalEndPointAssessmentPrice;
}
