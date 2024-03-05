namespace SFA.DAS.Apprenticeships.Web.Models
{
    public interface IRouteValuesProvider
    {
        public string? ApprenticeshipHashedId { get; set; }
        public long? ProviderReferenceNumber { get; set; }
    }
}
