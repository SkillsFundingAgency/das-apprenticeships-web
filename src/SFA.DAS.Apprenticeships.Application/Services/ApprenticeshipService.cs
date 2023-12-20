using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Requests;
using SFA.DAS.Apprenticeships.Domain.Interfaces;

namespace SFA.DAS.Apprenticeships.Application.Services
{
    public class ApprenticeshipService : IApprenticeshipService
    {
        private readonly IApiClient _apiClient;

        public ApprenticeshipService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<Guid> GetApprenticeshipKey(string apprenticeshipHashId)
        {
            var result = await _apiClient.Get<Guid>(new GetApprenticeshipKeyRequest(apprenticeshipHashId));
            return result.Body;
        }

        public async Task<ApprenticeshipPrice> GetApprenticeshipPrice(Guid apprenticeshipKey)
        {
            var result = await _apiClient.Get<ApprenticeshipPrice>(new GetApprenticeshipPriceRequest(apprenticeshipKey));
            return result.Body;
        }

        public async Task CreatePriceHistory(
            Guid apprenticeshipKey,
            long? providerId,
            long? employerId,
            string userId,
            decimal? trainingPrice,
            decimal? assessmentPrice,
            decimal? totalPrice,
            string reason,
            DateTime effectiveFromDate,
            string? providerApprovedBy)
        {
            await _apiClient.Post<object>(new CreateApprenticeshipPriceHistoryRequest(apprenticeshipKey,
                new CreateApprenticeshipPriceHistoryData
                {
                    ProviderId = providerId,
                    EmployerId = employerId,
                    UserId = userId,
                    TrainingPrice = trainingPrice,
                    AssessmentPrice = assessmentPrice,
                    TotalPrice = totalPrice,
                    Reason = reason,
                    EffectiveFromDate = effectiveFromDate,
                    ProviderApprovedBy = providerApprovedBy
                }));
        }

        public async Task<ApprenticeshipPrice> CreateApprenticeshipPriceHistory(Guid apprenticeshipKey)
        {
            var result = await _apiClient.Get<ApprenticeshipPrice>(new GetApprenticeshipPriceRequest(apprenticeshipKey));
            return result.Body;
        }
    }
}
