using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;

namespace SFA.DAS.Apprenticeships.Domain.Interfaces
{
    public interface IApprenticeshipService
    {
        Task<Guid> GetApprenticeshipKey(string apprenticeshipHashId);
        Task<ApprenticeshipPrice> GetApprenticeshipPrice(Guid apprenticeshipKey);

        Task CreatePriceHistory(
            Guid apprenticeshipKey,
            string initiator,
            string userId,
            decimal? trainingPrice,
            decimal? assessmentPrice,
            decimal? totalPrice,
            string? reason,
            DateTime effectiveFromDate);

        Task<GetPendingPriceChangeResponse> GetPendingPriceChange(Guid apprenticeshipKey);
        Task CancelPendingPriceChange(Guid apprenticeshipKey);
        Task RejectPendingPriceChange(Guid apprenticeshipKey, string reason);
        Task ApprovePendingPriceChange(Guid apprenticeshipKey, string userId);

    }
}
