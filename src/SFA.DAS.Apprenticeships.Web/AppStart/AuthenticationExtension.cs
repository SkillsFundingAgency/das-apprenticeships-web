using Microsoft.AspNetCore.Authentication.Cookies;
using SFA.DAS.Apprenticeships.Web.Infrastructure;
using SFA.DAS.DfESignIn.Auth.AppStart;
using SFA.DAS.DfESignIn.Auth.Enums;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.GovUK.Auth.AppStart;
using SFA.DAS.Provider.Shared.UI.Models;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Apprenticeships.Web.AppStart
{
    [ExcludeFromCodeCoverage]
    public static class AuthenticationExtension
    {
        public static void SetUpProviderAuthentication(this IServiceCollection services, ConfigurationManager config)
        {
            if (config.UseLocalStubAuth())
            {
                services.AddProviderStubAuthentication();
            }
            else if (config.UseDfeSignIn())
            {
                // Use DfESignIn OpenIdConnect
                services.AddAndConfigureDfESignInAuthentication(
                    config,
                    "SFA.DAS.ProviderApprenticeshipService",
                    typeof(CustomServiceRole),
                    ClientName.ProviderRoatp,
                    "/signout",
                    "");
            }
        }

        public static void SetUpEmployerAuthentication(this IServiceCollection services, ConfigurationManager config, ServiceParameters serviceParameters)
        {
            if (config.UseLocalStubAuth())
            {
                services.AddEmployerStubAuthentication();    
                services.AddAuthenticationCookie(serviceParameters.AuthenticationType);
                services.AddMaMenuConfiguration(RouteNames.EmployerSignOut, identityClientId: "no-auth-id", config["ResourceEnvironmentName"]);
            }
            else if (config.UseGovSignIn())
            {
                // Use GovSignIn OpenIdConnect
                services.AddAndConfigureGovUkAuthentication(config, typeof(EmployerAccountPostAuthenticationClaimsHandler), "", "/SignIn-Stub");
                //TODO NEEDED?
                services.AddMaMenuConfiguration(RouteNames.EmployerSignOut, config["ResourceEnvironmentName"]);
            }
            //TODO NEEDED?
            services.AddSingleton(new ProviderSharedUIConfiguration()); 
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
    }
}