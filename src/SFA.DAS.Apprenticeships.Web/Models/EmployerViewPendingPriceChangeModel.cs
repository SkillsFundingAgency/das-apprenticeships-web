using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;

namespace SFA.DAS.Apprenticeships.Web.Models;

public class EmployerViewPendingPriceChangeModel : ViewPendingPriceChangeModel
{
    public EmployerViewPendingPriceChangeModel(Guid apprenticeshipKey, string apprenticeshipHashedId,
        PendingPriceChange pendingPriceChange, long employerAccountId) : base(apprenticeshipKey, apprenticeshipHashedId, pendingPriceChange)
    {
        EmployerAccountId = employerAccountId;
    }
    public long? EmployerAccountId { get; set; }
}