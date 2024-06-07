using Microsoft.Extensions.Logging;
using SFA.DAS.Apprenticeships.Application.Exceptions;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Requests;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Responses;
using SFA.DAS.Apprenticeships.Domain.Interfaces;

namespace SFA.DAS.Apprenticeships.Application.Services;

public class ApprenticeshipService : IApprenticeshipService
{
    private readonly IApiClient _apiClient;
    private readonly ILogger<ApprenticeshipService> _logger;

    public ApprenticeshipService(IApiClient apiClient, ILogger<ApprenticeshipService> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task<Guid> GetApprenticeshipKey(string apprenticeshipHashId)
    {
        if (string.IsNullOrEmpty(apprenticeshipHashId))
        {
            _logger.LogWarning("Cannot get apprenticeshipKey when apprenticeshipHashId is null or empty");
            return Guid.Empty;
        }

        var result = await _apiClient.Get<Guid>(new GetApprenticeshipKeyRequest(apprenticeshipHashId));
        if (result == null || result.Body == Guid.Empty)
        {
            _logger.LogWarning("ApprenticeshipKey not found for apprenticeshipHashId {apprenticeshipHashId}", apprenticeshipHashId);
            return Guid.Empty;
        }

        return result.Body;
    }

    public async Task<ApprenticeshipPrice?> GetApprenticeshipPrice(string apprenticeshipHashId)
    {
        var apprenticeshipKey = await GetApprenticeshipKey(apprenticeshipHashId);

        if (apprenticeshipKey == Guid.Empty)
            return null;

        var result = await _apiClient.Get<ApprenticeshipPrice>(new GetApprenticeshipPriceRequest(apprenticeshipKey));

        if (result == null || result.Body == null)
        {
            _logger.LogWarning("ApprenticeshipPrice not found for apprenticeshipKey {apprenticeshipKey}", apprenticeshipKey);
            return null;
        }

        return result.Body;
    }

    public async Task<GetPendingPriceChangeResponse?> GetPendingPriceChange(string apprenticeshipHashId)
    {
        var apprenticeshipKey = await GetApprenticeshipKey(apprenticeshipHashId);

        if (apprenticeshipKey == Guid.Empty)
            return null;

        var result = await _apiClient.Get<GetPendingPriceChangeResponse>(new GetPendingPriceChangeRequest(apprenticeshipKey));

        if (result == null || result.Body == null || !result.Body.HasPendingPriceChange)
        {
            _logger.LogWarning("PendingPriceChange not found for apprenticeshipKey {apprenticeshipKey}", apprenticeshipKey);
            return null;
        }

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

    public async Task ApprovePendingStartDateChange(Guid apprenticeshipKey, string userId)
    {
        var response = await _apiClient.Patch<object>(new ApprovePendingStartDateChangeRequest(apprenticeshipKey, new ApprovePendingStartDateChangeData() { UserId = userId }));
        if (!string.IsNullOrEmpty(response?.ErrorContent))
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

    public async Task<ApprenticeshipStartDate?> GetApprenticeshipStartDate(string apprenticeshipHashId)
    {
        var apprenticeshipKey = await GetApprenticeshipKey(apprenticeshipHashId);

        if (apprenticeshipKey == Guid.Empty)
            return null;

        var result = await _apiClient.Get<ApprenticeshipStartDate>(new GetApprenticeshipStartDateRequest(apprenticeshipKey));

        if(result == null || result.Body == null)
        {
            _logger.LogWarning("ApprenticeshipStartDate not found for apprenticeshipKey {apprenticeshipKey}", apprenticeshipKey);
            return null;
        }

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

    public async Task CreateStartDateChange(Guid apprenticeshipKey, string initiator, string userId, string? reason, DateTime newActualStartDate, DateTime newPlannedEndDate)
    {
        var response = await _apiClient.Post<object>(new CreateChangeOfStartDateRequest(apprenticeshipKey,
            new CreateChangeOfStartDateData
            {
            Initiator = initiator,
            UserId = userId,
            Reason = reason,
            ActualStartDate = newActualStartDate,
            PlannedEndDate = newPlannedEndDate
            }));

        if (!string.IsNullOrEmpty(response.ErrorContent))
        {
            throw new ServiceException(response.ErrorContent);
        }
    }

    public async Task<GetPendingStartDateChangeResponse?> GetPendingStartDateChange(string apprenticeshipHashId)
    {
        var apprenticeshipKey = await GetApprenticeshipKey(apprenticeshipHashId);

        if (apprenticeshipKey == Guid.Empty)
            return null;

        var result = await _apiClient.Get<GetPendingStartDateChangeResponse>(new GetPendingStartDateChangeRequest(apprenticeshipKey));

        if (result == null || result.Body == null || !result.Body.HasPendingStartDateChange)
        {
            _logger.LogWarning("PendingStartDateChange not found for apprenticeshipKey {apprenticeshipKey}", apprenticeshipKey);
            return null;
        }

        return result.Body;
	}

    public async Task RejectPendingStartDateChange(Guid apprenticeshipKey, string reason)
    {
	    var response = await _apiClient.Patch<object>(new RejectPendingStartDateChangeRequest(apprenticeshipKey, new RejectPendingStartDateChangeData { Reason = reason }));
	    if (!string.IsNullOrEmpty(response?.ErrorContent))
	    {
		    throw new ServiceException(response.ErrorContent);
	    }
    }

    public async Task CancelPendingStartDateChange(Guid apprenticeshipKey)
    {
		var response = await _apiClient.Delete<object>(new CancelPendingStartDateChangeRequest(apprenticeshipKey));
		if (!string.IsNullOrEmpty(response.ErrorContent))
		{
			throw new ServiceException(response.ErrorContent);
		}
	}

    public async Task FreezePayments(Guid apprenticeshipKey, string? reason)
    {
        var response = await _apiClient.Post<object>(new FreezePaymentsRequest(apprenticeshipKey, new FreezePaymentsData{ Reason = reason}));
        if (!string.IsNullOrEmpty(response.ErrorContent))
        {
            throw new ServiceException(response.ErrorContent);
        }
    }

    public async Task UnfreezePayments(Guid apprenticeshipKey)
    {
        var response = await _apiClient.Post<object>(new UnfreezePaymentsRequest(apprenticeshipKey));
        if (!string.IsNullOrEmpty(response.ErrorContent))
        {
            throw new ServiceException(response.ErrorContent);
        }
    }
}
