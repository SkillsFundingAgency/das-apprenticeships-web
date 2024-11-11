using SFA.DAS.Apprenticeships.Web.Infrastructure;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.Apprenticeships.Web.Middleware;

namespace SFA.DAS.Apprenticeships.Web.AppStart;

[ExcludeFromCodeCoverage]
public static class AuthorizationExtension
{
    private const string ProviderDaa = "DAA";
    private const string ProviderDab = "DAB";
    private const string ProviderDac = "DAC";
    private const string ProviderDav = "DAV";

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

    public static void AddAuthorizationPolicies(this IServiceCollection services)
    {
        FailedStartUpMiddleware.StartupStep = "AddAuthorizationPolicies";
        services.AddAuthorization(options =>
        {
            options.AddPolicy(PolicyNames.IsAuthenticated, policy =>
            {
                policy.RequireAuthenticatedUser();
            });
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
                    policy.RequireClaim(ProviderClaims.Service, ProviderDaa, ProviderDab, ProviderDac, ProviderDav);
                    policy.Requirements.Add(new ProviderAccountRequirement());
                    policy.RequireAuthenticatedUser();
                });
        });
    }
}