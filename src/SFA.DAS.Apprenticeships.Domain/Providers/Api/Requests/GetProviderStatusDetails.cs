using SFA.DAS.Apprenticeships.Domain.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Apprenticeships.Domain.Providers.Api.Requests;

[ExcludeFromCodeCoverage]
public class GetProviderStatusDetails : IGetApiRequest
{
    private readonly long _ukprn;

    public GetProviderStatusDetails(long ukprn)
    {
        _ukprn = ukprn;
    }

    public string GetUrl => $"provideraccounts/{_ukprn}";
    public bool SendBearerToken => true;
}