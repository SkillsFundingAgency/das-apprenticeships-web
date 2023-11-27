namespace SFA.DAS.Apprenticeships.Web.Models
{
    public interface IChangeOfPriceModel
    {
        public DateTime? ApprenticeshipActualStartDate { get; set; }
        public DateTime? ApprenticeshipPlannedEndDate { get; set; }
        public DateField EffectiveFromDate { get; set; }
        public string? ReasonForChangeOfPrice { get; set; }
        public string? EmployerName { get; set; } // PR talking point, should we call this NotificationContactName (or similar)? then it can be used for both provider and employer journeys
    }
}
