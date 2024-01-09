using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;

namespace SFA.DAS.Apprenticeships.Web.Models;

public class EmployerViewPendingPriceChangeModel : ViewPendingPriceChangeModel
{
    public EmployerViewPendingPriceChangeModel(Guid apprenticeshipKey, string apprenticeshipHashedId,
        PendingPriceChange pendingPriceChange, long employerAccountId, string providerName, string backLinkUrl) : base(apprenticeshipKey, apprenticeshipHashedId, pendingPriceChange)
    {
        EmployerAccountId = employerAccountId;
        ProviderName = providerName;
        BackLinkUrl = backLinkUrl;
    }
    public long? EmployerAccountId { get; set; }
    public string ProviderName { get; set; }
    public string BackLinkUrl { get; set; }
}