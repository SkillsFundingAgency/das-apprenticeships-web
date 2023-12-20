using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;

namespace SFA.DAS.Apprenticeships.Domain.Interfaces
{
    public interface IApprenticeshipService
    {
        Task<Guid> GetApprenticeshipKey(string apprenticeshipHashId);
        Task<ApprenticeshipPrice> GetApprenticeshipPrice(Guid apprenticeshipKey);

        Task CreatePriceHistory(
            Guid apprenticeshipKey,
            long? providerId,
            long? employerId,
            string userId,
            decimal? trainingPrice,
            decimal? assessmentPrice,
            decimal? totalPrice,
            string reason,
            DateTime effectiveFromDate);

        Task<GetPendingPriceChangeResponse> GetPendingPriceChange(Guid apprenticeshipKey);
	}
}
