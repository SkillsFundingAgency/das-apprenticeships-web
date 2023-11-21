using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;

namespace SFA.DAS.Apprenticeships.Domain.Interfaces
{
    public interface IApprenticeshipService
    {
        Task<Guid> GetApprenticeshipKey(string apprenticeshipHashId);
        Task<ApprenticeshipPrice> GetApprenticeshipPrice(Guid apprenticeshipKey);
    }
}
