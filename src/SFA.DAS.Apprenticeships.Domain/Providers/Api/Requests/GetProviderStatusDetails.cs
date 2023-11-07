using SFA.DAS.Apprenticeships.Domain.Interfaces;

namespace SFA.DAS.Apprenticeships.Domain.Providers.Api.Requests
{
    public class GetProviderStatusDetails : IGetApiRequest
    {
        private readonly long _ukprn;

        public GetProviderStatusDetails(long ukprn)
        {
            _ukprn = ukprn;
        }

        public string GetUrl => $"provideraccounts/{_ukprn}";
    }
}
