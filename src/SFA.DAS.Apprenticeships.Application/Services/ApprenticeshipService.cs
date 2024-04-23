using SFA.DAS.Apprenticeships.Application.Exceptions;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Requests;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Responses;
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
            var response = await _apiClient.Delete<object>(new CancelPendingPriceChangeRequest(apprenticeshipKey));
            if (!string.IsNullOrEmpty(response.ErrorContent))
            {
                throw new ServiceException(response.ErrorContent);
            }
        }

        public async Task RejectPendingPriceChange(Guid apprenticeshipKey, string reason)
        {
            var response = await _apiClient.Patch<object>(new RejectPendingPriceChangeRequest(apprenticeshipKey, new RejectPendingPriceChangeData { Reason = reason }));
            if (!string.IsNullOrEmpty(response?.ErrorContent))
            {
                throw new ServiceException(response.ErrorContent);
            }
        }

        public async Task ApprovePendingPriceChange(Guid apprenticeshipKey, string userId)
        {
            var response = await _apiClient.Patch<object>(new ApprovePendingPriceChangeRequest(apprenticeshipKey, new ApprovePendingPriceChangeData { UserId = userId }));
            if (!string.IsNullOrEmpty(response?.ErrorContent))
            {
                throw new ServiceException(response.ErrorContent);
            }
        }

        public async Task ApprovePendingPriceChange(Guid apprenticeshipKey, string userId, decimal trainingPrice, decimal endPointAssessmentPrice)
        {
            var response = await _apiClient.Patch<object>(new ApprovePendingPriceChangeRequest(apprenticeshipKey, new ApprovePendingPriceChangeData { UserId = userId, TrainingPrice = trainingPrice, AssessmentPrice = endPointAssessmentPrice }));
            if (!string.IsNullOrEmpty(response?.ErrorContent))
            {
                throw new ServiceException(response.ErrorContent);
            }
        }

        public async Task<ApprenticeshipStartDate> GetApprenticeshipStartDate(Guid apprenticeshipKey)
        {
            var result = await _apiClient.Get<ApprenticeshipStartDate>(new GetApprenticeshipStartDateRequest(apprenticeshipKey));
            return result.Body;
        }

        public async Task<string> CreatePriceHistory(
            Guid apprenticeshipKey,
            string initiator,
            string userId,
            decimal? trainingPrice,
            decimal? assessmentPrice,
            decimal? totalPrice,
            string? reason,
            DateTime effectiveFromDate)
        {
			var response = await _apiClient.Post<PostPendingPriceChangeResponse>(new CreateApprenticeshipPriceHistoryRequest(apprenticeshipKey,
                new CreateApprenticeshipPriceHistoryData
                {
                    Initiator = initiator,
                    UserId = userId,
                    TrainingPrice = trainingPrice,
                    AssessmentPrice = assessmentPrice,
                    TotalPrice = totalPrice,
                    Reason = reason,
                    EffectiveFromDate = effectiveFromDate
                }));

			if (!string.IsNullOrEmpty(response.ErrorContent))
			{
				throw new ServiceException(response.ErrorContent);
			}

			return response.Body.PriceChangeStatus;
        }

        public async Task CreateStartDateChange(Guid apprenticeshipKey, string initiator, string userId, string? reason, DateTime newActualStartDate)
        {
            var response = await _apiClient.Post<object>(new CreateChangeOfStartDateRequest(apprenticeshipKey,
                new CreateChangeOfStartDateData
                {
                    Initiator = initiator,
                    UserId = userId,
                    Reason = reason,
                    ActualStartDate = newActualStartDate
                }));

            if (!string.IsNullOrEmpty(response.ErrorContent))
            {
                throw new ServiceException(response.ErrorContent);
            }
        }

        public async Task<GetPendingStartDateChangeResponse> GetPendingStartDateChange(Guid apprenticeshipKey)
        {
            var result = await _apiClient.Get<GetPendingStartDateChangeResponse>(new GetPendingStartDateChangeRequest(apprenticeshipKey));
            return result.Body;
        }

        public async Task ApprovePendingStartDateChange(Guid apprenticeshipKey, string userId)
        {
            var response = await _apiClient.Patch<object>(new ApprovePendingStartDateChangeRequest(apprenticeshipKey, new ApprovePendingPriceChangeData { UserId = userId }));
            if (!string.IsNullOrEmpty(response?.ErrorContent))
            {
                throw new ServiceException(response.ErrorContent);
            }
        }
    }
}
