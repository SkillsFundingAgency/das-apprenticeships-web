using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;
using SFA.DAS.Apprenticeships.Domain.Interfaces;

namespace SFA.DAS.Apprenticeships.Application.Services
{
    public class ApprenticeshipService : IApprenticeshipService
    {
        public Task<ApprenticeshipPrice> GetApprenticeshipPrice(string apprenticeshipId)
        {
            var temp = new ApprenticeshipPrice
            {
                FundingBandMaximum = 10000,
                AssessmentPrice = 1000,
                TrainingPrice = 8000
            };
            return Task.FromResult(temp);
        }
    }
}
