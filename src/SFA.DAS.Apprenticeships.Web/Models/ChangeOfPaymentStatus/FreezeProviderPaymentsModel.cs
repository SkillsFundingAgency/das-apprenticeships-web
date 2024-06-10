namespace SFA.DAS.Apprenticeships.Web.Models.ChangeOfPaymentStatus;

public class FreezeProviderPaymentsModel : ICacheModel
{
    public string EmployerAccountId { get; set; }
    public string? ApprenticeshipHashedId { get; set; }
    public string BackLinkUrl { get; set; }
    public Guid ApprenticeshipKey { get; set; }
    public string? ReasonForFreeze { get; set; }
    public bool? FreezePayments { get; set; }
    public string? CacheKey { get; set; }
}