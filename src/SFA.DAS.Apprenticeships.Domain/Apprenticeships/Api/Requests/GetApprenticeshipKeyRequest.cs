using SFA.DAS.Apprenticeships.Domain.Interfaces;

namespace SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Requests
{
    public class GetApprenticeshipKeyRequest : IGetApiRequest
    {
        private readonly string _apprenticeshipHashId;

        public GetApprenticeshipKeyRequest(string apprenticeshipHashId)
        {
            _apprenticeshipHashId = apprenticeshipHashId;
        }

        public string GetUrl => $"/Apprenticeship/{_apprenticeshipHashId}/key";
    }
}
