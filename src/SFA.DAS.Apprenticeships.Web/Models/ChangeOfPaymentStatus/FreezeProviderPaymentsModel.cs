namespace SFA.DAS.Apprenticeships.Web.Models.ChangeOfPaymentStatus;

public class UnfreezeProviderPaymentsModel : ICacheModel
{
    public string EmployerAccountId { get; set; } = string.Empty;
    public string ApprenticeshipHashedId { get; set; } = string.Empty;
    public string BackLinkUrl { get; set; } = string.Empty;
    public Guid ApprenticeshipKey { get; set; }
    public bool? UnfreezePayments { get; set; }
    public string? CacheKey { get; set; }
}