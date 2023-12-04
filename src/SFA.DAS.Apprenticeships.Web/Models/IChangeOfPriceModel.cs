namespace SFA.DAS.Apprenticeships.Web.Models
{
    public interface IChangeOfPriceModel
    {
        public DateTime? ApprenticeshipActualStartDate { get; set; }
        public DateTime? ApprenticeshipPlannedEndDate { get; set; }
        public DateField EffectiveFromDate { get; set; }
    }
}
