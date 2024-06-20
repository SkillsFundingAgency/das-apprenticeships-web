using SFA.DAS.Apprenticeships.Domain.Interfaces;

namespace SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Requests;

public class UnfreezePaymentsRequest : IPostApiRequest
{
    private readonly Guid _apprenticeshipKey;

    public UnfreezePaymentsRequest(Guid apprenticeshipKey)
    {
        _apprenticeshipKey = apprenticeshipKey;
    }

    public string PostUrl => $"Apprenticeship/{_apprenticeshipKey}/unfreeze";
    public bool SendBearerToken => true;
    public object Data { get; set; }
}
