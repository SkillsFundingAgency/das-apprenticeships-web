using SFA.DAS.Apprenticeships.Domain.Employers;
using SFA.DAS.Apprenticeships.Domain.Employers.Api.Requests;
using SFA.DAS.Apprenticeships.Domain.Employers.Api.Responses;
using SFA.DAS.Apprenticeships.Domain.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Apprenticeships.Application.Employer.Services;

[ExcludeFromCodeCoverage]
public class EmployerAccountService : IEmployerAccountService
{
    private readonly IApiClient _apiClient;

    public EmployerAccountService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }
    public async Task<EmployerUserAccounts> GetUserAccounts(string userId, string email)
    {
        var result = await _apiClient.Get<GetUserAccountsResponse>(new GetUserAccountsRequest(userId, email));

        return result.Body;
    }
}