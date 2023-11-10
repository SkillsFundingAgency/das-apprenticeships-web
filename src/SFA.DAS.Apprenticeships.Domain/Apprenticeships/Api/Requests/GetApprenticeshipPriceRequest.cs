using SFA.DAS.Apprenticeships.Domain.Interfaces;

namespace SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Requests
{
    public class GetApprenticeshipPriceRequest : IGetApiRequest
    {
        private readonly string _apprenticeshipKey;

        public GetApprenticeshipPriceRequest(string apprenticeshipKey)
        {
            _apprenticeshipKey = apprenticeshipKey;
        }

        public string GetUrl => $"/Apprenticeship/{_apprenticeshipKey}/price";
    }
}
