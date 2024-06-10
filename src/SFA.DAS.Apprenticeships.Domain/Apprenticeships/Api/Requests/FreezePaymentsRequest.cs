using SFA.DAS.Apprenticeships.Domain.Interfaces;

namespace SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Requests;

public class FreezePaymentsRequest : IPostApiRequest
{
    private readonly Guid _apprenticeshipKey;

    public FreezePaymentsRequest(Guid apprenticeshipKey, FreezePaymentsData data)
    {
        _apprenticeshipKey = apprenticeshipKey;
        Data = data;
    }

    public string PostUrl => $"Apprenticeship/{_apprenticeshipKey}/freeze";
    public bool SendBearerToken => true;
    public object Data { get; set; }
}

public class FreezePaymentsData
{
    public string? Reason { get; set; }
}