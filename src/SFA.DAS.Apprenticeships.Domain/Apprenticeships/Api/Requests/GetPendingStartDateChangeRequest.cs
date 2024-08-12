using SFA.DAS.Apprenticeships.Domain.Interfaces;

namespace SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Requests;

public class GetPendingStartDateChangeRequest : IGetApiRequest
{
    private readonly Guid _apprenticeshipKey;

    public GetPendingStartDateChangeRequest(Guid apprenticeshipKey)
    {
        _apprenticeshipKey = apprenticeshipKey;
    }

    public string GetUrl => $"Apprenticeship/{_apprenticeshipKey}/startDateChange/pending";
    public bool SendBearerToken => true;
}