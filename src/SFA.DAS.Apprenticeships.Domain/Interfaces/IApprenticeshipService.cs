using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Responses;

namespace SFA.DAS.Apprenticeships.Domain.Interfaces
{
    public interface IApprenticeshipService
    {
        Task<Guid> GetApprenticeshipKey(string apprenticeshipHashId);
        Task<ApprenticeshipPrice?> GetApprenticeshipPrice(string apprenticeshipHashId);

        Task<string> CreatePriceHistory(
            Guid apprenticeshipKey,
            string initiator,
            string userId,
            decimal? trainingPrice,
            decimal? assessmentPrice,
            decimal? totalPrice,
            string? reason,
            DateTime effectiveFromDate);

        Task<GetPendingPriceChangeResponse?> GetPendingPriceChange(string apprenticeshipHashId);
        Task CancelPendingPriceChange(Guid apprenticeshipKey);
        Task RejectPendingPriceChange(Guid apprenticeshipKey, string reason);
        Task ApprovePendingPriceChange(Guid apprenticeshipKey, string userId);
        Task ApprovePendingPriceChange(Guid apprenticeshipKey, string userId, decimal trainingPrice, decimal endPointAssessmentPrice);
        Task<ApprenticeshipStartDate?> GetApprenticeshipStartDate(string apprenticeshipHashId);
        Task CreateStartDateChange(Guid apprenticeshipKey, string initiator, string userId, string? reason, DateTime newActualStartDate, DateTime newPlannedEndDate);
        Task<GetPendingStartDateChangeResponse?> GetPendingStartDateChange(string apprenticeshipHashId);
        Task ApprovePendingStartDateChange(Guid apprenticeshipKey, string userId);
        Task RejectPendingStartDateChange(Guid apprenticeshipKey, string reason);

	}
}
