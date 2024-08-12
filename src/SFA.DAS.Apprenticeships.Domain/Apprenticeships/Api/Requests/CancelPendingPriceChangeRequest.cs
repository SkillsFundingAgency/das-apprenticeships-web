using SFA.DAS.Apprenticeships.Domain.Interfaces;

namespace SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Requests
{
    public class CancelPendingPriceChangeRequest : IDeleteApiRequest
    {
        private readonly Guid _apprenticeshipKey;

        public CancelPendingPriceChangeRequest(Guid apprenticeshipKey)
        {
            _apprenticeshipKey = apprenticeshipKey;
        }

        public string DeleteUrl => $"Apprenticeship/{_apprenticeshipKey}/priceHistory/pending";
        public bool SendBearerToken => true;
    }
}
