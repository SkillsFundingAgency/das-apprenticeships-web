using SFA.DAS.Apprenticeships.Web.Infrastructure;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;

namespace SFA.DAS.Apprenticeships.Web.AppStart
{
    [ExcludeFromCodeCoverage]
    public static class AuthorizationExtension
    {
        private const string _providerDaa = "DAA";
        private const string _providerDab = "DAB";
        private const string _providerDac = "DAC";
        private const string _providerDav = "DAV";

        public static void SetUpProviderAuthorizationServices(this IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationHandler, ProviderAccountAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, TrainingProviderAllRolesAuthorizationHandler>();
            services.AddSingleton<ITrainingProviderAuthorizationHandler, TrainingProviderAuthorizationHandler>();
        }

        public static void SetUpEmployerAuthorizationServices(this IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationHandler, EmployerAccountAuthorizationHandler>();
        }

        public static void AddSharedAuthenticationServices(this IServiceCollection services)
        {
            //TODO Check with Dan - needed?
            //services.AddTransient<IAuthorizationHandler, EmployerAccountAuthorizationHandler>();
            //services.AddTransient<IAuthorizationHandler, ProviderAccountAuthorizationHandler>();
            //services.AddSingleton<ITrainingProviderAuthorizationHandler, TrainingProviderAuthorizationHandler>();
            //services.AddSingleton<IAuthorizationHandler, TrainingProviderAllRolesAuthorizationHandler>();
        }

        public static void AddAuthorizationService(this IServiceCollection services,
            AuthenticationType? serviceParametersAuthenticationType)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    PolicyNames
                        .HasEmployerAccount
                    , policy =>
                    {
                        policy.RequireClaim(EmployerClaims.AccountsClaimsTypeIdentifier);
                        policy.Requirements.Add(new EmployerAccountRequirement());
                        policy.RequireAuthenticatedUser();
                    });
                options.AddPolicy(
                    PolicyNames
                        .HasProviderAccount
                    , policy =>
                    {
                     
                        policy.RequireClaim(ProviderClaims.ProviderUkprn);
                        policy.RequireClaim(ProviderClaims.Service, _providerDaa, _providerDab, _providerDac, _providerDav);
                        policy.Requirements.Add(new ProviderAccountRequirement());
                        policy.Requirements.Add(new TrainingProviderAllRolesRequirement());
                        policy.RequireAuthenticatedUser();
                    });
            });
        }
    }
}