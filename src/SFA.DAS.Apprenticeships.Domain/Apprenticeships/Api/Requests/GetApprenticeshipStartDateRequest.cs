using SFA.DAS.Apprenticeships.Domain.Interfaces;

namespace SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Requests;

public class GetApprenticeshipStartDateRequest : IGetApiRequest
{
    private readonly Guid _apprenticeshipKey;

    public GetApprenticeshipStartDateRequest(Guid apprenticeshipKey)
    {
        _apprenticeshipKey = apprenticeshipKey;
    }

    public string GetUrl => $"Apprenticeship/{_apprenticeshipKey}/startDate";
}