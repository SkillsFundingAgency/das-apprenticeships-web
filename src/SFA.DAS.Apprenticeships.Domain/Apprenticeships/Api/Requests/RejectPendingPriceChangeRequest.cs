using SFA.DAS.Apprenticeships.Domain.Interfaces;

namespace SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Requests;

public class RejectPendingPriceChangeRequest : IPatchApiRequest
{
    private readonly Guid _apprenticeshipKey;

    public RejectPendingPriceChangeRequest(Guid apprenticeshipKey, RejectPendingPriceChangeData data)
    {
        _apprenticeshipKey = apprenticeshipKey;
        Data = data;
    }

    public string PatchUrl => $"Apprenticeship/{_apprenticeshipKey}/priceHistory/pending/reject";
    public object Data { get; set; }
}

public class RejectPendingPriceChangeData
{
    public string Reason { get; set; }
}