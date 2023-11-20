using System.Diagnostics.CodeAnalysis;
using SFA.DAS.Apprenticeships.Domain.Interfaces;
using SFA.DAS.Apprenticeships.Domain.Providers.Api.Requests;
using SFA.DAS.Apprenticeships.Domain.Providers.Api.Responses;

namespace SFA.DAS.Apprenticeships.Application.Provider.Services
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public class TrainingProviderService : ITrainingProviderService
    {
        private readonly IApiClient _apiClient;

        public TrainingProviderService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }
        public async Task<bool> CanProviderAccessService(long ukprn)
        {
            var response = await _apiClient.Get<ProviderAccountResponse>(new GetProviderStatusDetails(ukprn));
            return response.Body.CanAccessService;
        }

    }
}
