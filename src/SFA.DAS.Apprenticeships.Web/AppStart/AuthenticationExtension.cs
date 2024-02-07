using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using SFA.DAS.Apprenticeships.Web.Identity.Authentication;
using SFA.DAS.Apprenticeships.Web.Infrastructure;
using SFA.DAS.DfESignIn.Auth.AppStart;
using SFA.DAS.DfESignIn.Auth.Enums;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.GovUK.Auth.AppStart;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Apprenticeships.Web.AppStart
{
    [ExcludeFromCodeCoverage]
    public static class AuthenticationExtension
    {
        private const string ProviderCookieAuthName = "SFA.DAS.ProviderApprenticeshipService";
        public static void SetUpProviderAuthentication(this IServiceCollection services, ConfigurationManager config)
        {
            if (config.UseStubAuth())
            {
                services.AddProviderStubAuthentication();
            }
            else if (config.UseDfeSignIn())
            {
                // Use DfESignIn OpenIdConnect
                services.AddAndConfigureDfESignInAuthentication(
                    config,
                    ProviderCookieAuthName,
                    typeof(CustomServiceRole),
                    ClientName.ProviderRoatp,
                    "/signout", // This will set the cookie signout route value (note: naming is misleading in the nuget package)
                    ""); // This will redirect the user to the PAS home page after being signed out
            }
        }

        public static void SetUpEmployerAuthentication(this IServiceCollection services, ConfigurationManager config, ServiceParameters serviceParameters)
        {
            if (config.UseStubAuth())
            {
                services.AddEmployerStubAuthentication();    
                services.AddAuthenticationCookie(serviceParameters.AuthenticationType);
                services.AddMaMenuConfiguration(RouteNames.EmployerSignOut, identityClientId: "no-auth-id", config["ResourceEnvironmentName"]);
            }
            else if (config.UseGovSignIn())
            {
                // Use GovSignIn OpenIdConnect
                services.AddAndConfigureGovUkAuthentication(config, typeof(EmployerAccountPostAuthenticationClaimsHandler), "", "/SignIn-Stub");
                services.AddMaMenuConfiguration(RouteNames.EmployerSignOut, config["ResourceEnvironmentName"]);
            }
        }
        private static void AddAuthenticationCookie(this IServiceCollection services,
            AuthenticationType? serviceParametersAuthenticationType)
        {
            services.AddAuthentication().AddCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/error/403");
                options.ExpireTimeSpan = TimeSpan.FromHours(1);
                options.Cookie.Name = $"SFA.DAS.Apprenticeships.Web.{serviceParametersAuthenticationType}Auth";
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.SlidingExpiration = true;
                options.Cookie.SameSite = SameSiteMode.None;
                options.CookieManager = new ChunkingCookieManager { ChunkSize = 3000 };
            });
        }

        private static void AddProviderStubAuthentication(this IServiceCollection services)
        {
            services
                .AddAuthentication("Provider-stub")
                .AddScheme<AuthenticationSchemeOptions, ProviderStubAuthHandler>(
                "Provider-stub",
                options => { });
        }

        private static void AddEmployerStubAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication("Employer-stub").AddScheme<AuthenticationSchemeOptions, EmployerStubAuthHandler>(
                "Employer-stub",
                options => { });
        }
    }
}