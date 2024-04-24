using SFA.DAS.Apprenticeships.Domain.Interfaces;

namespace SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Requests
{
    public class GetApprenticeshipPriceRequest : IGetApiRequest
    {
        private readonly Guid _apprenticeshipKey;

        public GetApprenticeshipPriceRequest(Guid apprenticeshipKey)
        {
            _apprenticeshipKey = apprenticeshipKey;
        }

        public string GetUrl => $"Apprenticeship/{_apprenticeshipKey}/price";
        public bool SendBearerToken => true;
    }
}
