using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;

namespace SFA.DAS.Apprenticeships.Domain.Interfaces
{
    public interface IApprenticeshipService
    {
        Task<ApprenticeshipPrice> GetApprenticeshipPrice(string apprenticeshipId);
    }
}
