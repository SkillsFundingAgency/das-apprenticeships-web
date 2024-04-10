using SFA.DAS.Apprenticeships.Domain.Interfaces;

namespace SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Requests
{
    public class GetPendingPriceChangeRequest : IGetApiRequest
    {
        private readonly Guid _apprenticeshipKey;

        public GetPendingPriceChangeRequest(Guid apprenticeshipKey)
        {
            _apprenticeshipKey = apprenticeshipKey;
        }

        public string GetUrl => $"Apprenticeship/{_apprenticeshipKey}/priceHistory/pending";
        public bool SendBearerToken => true;
    }
}
