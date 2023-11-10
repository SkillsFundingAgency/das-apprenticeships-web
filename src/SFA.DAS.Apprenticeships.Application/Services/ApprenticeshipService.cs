using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Requests;
using SFA.DAS.Apprenticeships.Domain.Interfaces;

namespace SFA.DAS.Apprenticeships.Application.Services
{
    public class ApprenticeshipService : IApprenticeshipService
    {
        private readonly IApiClient _apiClient;
        private const bool UseStub = false; // DO NOT APPROVE PR WITH THIS STILL HERE

        public ApprenticeshipService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<string> GetApprenticeshipKey(string apprenticeshipHashId)
        {
            var result = await _apiClient.Get<string>(new GetApprenticeshipKeyRequest(apprenticeshipHashId));
            return result.Body;
        }

        public async Task<ApprenticeshipPrice> GetApprenticeshipPrice(string apprenticeshipKey)
        {
            if(UseStub)
            {
                return await StubGetApprenticeshipPrice(apprenticeshipKey);
            }

            var result = await _apiClient.Get<ApprenticeshipPrice>(new GetApprenticeshipPriceRequest(apprenticeshipKey));
            return result.Body;
        }

        // DO NOT APPROVE PR WITH THIS STILL HERE
        public Task<ApprenticeshipPrice> StubGetApprenticeshipPrice(string apprenticeshipKey)
        {
            var temp = new ApprenticeshipPrice
            {
                FundingBandMaximum = 10000,
                AssessmentPrice = 1000,
                TrainingPrice = 8000
            };
            return Task.FromResult(temp);
        }


    }
}
