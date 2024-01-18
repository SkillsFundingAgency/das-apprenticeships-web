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

        public async Task<GetPendingPriceChangeResponse> GetPendingPriceChange(Guid apprenticeshipKey)
        {
	        var result = await _apiClient.Get<GetPendingPriceChangeResponse>(new GetPendingPriceChangeRequest(apprenticeshipKey));
	        return result.Body;
        }

        public async Task CancelPendingPriceChange(Guid apprenticeshipKey)
        {
            await _apiClient.Delete<object>(new CancelPendingPriceChangeRequest(apprenticeshipKey));
        }

        public async Task RejectPendingPriceChange(Guid apprenticeshipKey, string reason)
        {
            await _apiClient.Patch<object>(new RejectPendingPriceChangeRequest(apprenticeshipKey, new RejectPendingPriceChangeData { Reason = reason }));
        }

        public async Task ApprovePendingPriceChange(Guid apprenticeshipKey, string userId)
        {
			await _apiClient.Patch<object>(new ApprovePendingPriceChangeRequest(apprenticeshipKey, new ApprovePendingPriceChangeData { UserId = userId }));
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
            DateTime effectiveFromDate)
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
                    EffectiveFromDate = effectiveFromDate
                }));
        }

        public async Task<GetPendingPriceChangeResponse> CreateApprenticeshipPriceHistory(Guid apprenticeshipKey)
        {
            var result = await _apiClient.Get<GetPendingPriceChangeResponse>(new GetApprenticeshipPriceRequest(apprenticeshipKey));
            return result.Body;
        }
    }
}
