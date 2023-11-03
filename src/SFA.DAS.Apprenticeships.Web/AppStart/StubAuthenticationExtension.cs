using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authentication;

namespace SFA.DAS.Apprenticeships.Web.AppStart
{
    [ExcludeFromCodeCoverage]
    public static class StubAuthenticationExtension
    {
        public static void AddProviderStubAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication("Provider-stub").AddScheme<AuthenticationSchemeOptions, ProviderStubAuthHandler>(
                "Provider-stub",
                options => { });
        }

        public static void AddEmployerStubAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication("Employer-stub").AddScheme<AuthenticationSchemeOptions, EmployerStubAuthHandler>(
                "Employer-stub",
                options => { });
        }
    }
}