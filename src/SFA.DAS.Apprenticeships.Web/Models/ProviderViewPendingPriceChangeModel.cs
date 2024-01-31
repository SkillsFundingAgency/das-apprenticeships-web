using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;

namespace SFA.DAS.Apprenticeships.Web.Models;

public class ProviderViewPendingPriceChangeModel : ViewPendingPriceChangeModel
{
    public ProviderViewPendingPriceChangeModel(Guid apprenticeshipKey, string apprenticeshipHashedId,
        PendingPriceChange pendingPriceChange, long providerReferenceNumber) : base(apprenticeshipKey, apprenticeshipHashedId, pendingPriceChange)
    {
        ProviderReferenceNumber = providerReferenceNumber;
    }
    public long ProviderReferenceNumber { get; set; }
}