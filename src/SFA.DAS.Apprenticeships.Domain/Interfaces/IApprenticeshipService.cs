using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;

namespace SFA.DAS.Apprenticeships.Domain.Interfaces
{
    public interface IApprenticeshipService
    {
        Task<string> GetApprenticeshipKey(string apprenticeshipHashId);
        Task<ApprenticeshipPrice> GetApprenticeshipPrice(string apprenticeshipKey);
    }
}
