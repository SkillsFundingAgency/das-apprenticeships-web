﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using SFA.DAS.Apprenticeships.Web.Identity.Authentication;
using SFA.DAS.Apprenticeships.Web.Infrastructure;
using SFA.DAS.DfESignIn.Auth.AppStart;
using SFA.DAS.DfESignIn.Auth.Enums;
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
                    "/service/signout", // This will set the cookie signout route value (note: naming is misleading in the nuget package)
                    ""); // This will redirect the user to the PAS home page after being signed out
            }
        }

        public static void SetUpEmployerAuthentication(this IServiceCollection services, ConfigurationManager config, ServiceParameters serviceParameters)
        {
            services.AddAndConfigureGovUkAuthentication(config, typeof(EmployerAccountPostAuthenticationClaimsHandler), "", "/service/SignIn-Stub");
        }

        private static void AddProviderStubAuthentication(this IServiceCollection services)
        {
            services
                .AddAuthentication("Provider-stub")
                .AddScheme<AuthenticationSchemeOptions, ProviderStubAuthHandler>(
                "Provider-stub",
                options => { });
        }
    }
}