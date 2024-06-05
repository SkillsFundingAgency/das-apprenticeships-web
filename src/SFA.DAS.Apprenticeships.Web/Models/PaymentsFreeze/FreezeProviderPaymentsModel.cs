namespace SFA.DAS.Apprenticeships.Web.Models.PaymentsFreeze;

public class FreezeProviderPaymentsModel
{
    public string EmployerAccountId { get; set; }
    public string? ApprenticeshipHashedId { get; set; }
    public string BackLinkUrl { get; set; }
    public Guid ApprenticeshipKey { get; set; }
    public string? ReasonForFreeze { get; set; }
    public bool? FreezePayments { get; set; }
}