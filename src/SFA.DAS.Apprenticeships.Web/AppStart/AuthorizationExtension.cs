using SFA.DAS.Apprenticeships.Web.Infrastructure;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;

namespace SFA.DAS.Apprenticeships.Web.AppStart
{
    [ExcludeFromCodeCoverage]
    public static class AuthorizationExtension
    {
        public static void SetUpProviderAuthorizationServices(this IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationHandler, ProviderAccountAuthorizationHandler>();
            //TODO CHECK IF THESE ARE NEEDED
            //services.AddSingleton<ITrainingProviderAuthorizationHandler, TrainingProviderAuthorizationHandler>();
            //services.AddSingleton<IAuthorizationHandler, TrainingProviderAllRolesAuthorizationHandler>();
        }

        public static void SetUpEmployerAuthorizationServices(this IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationHandler, EmployerAccountAuthorizationHandler>();
            //TODO CHECK IF THESE ARE NEEDED
            //services.AddSingleton<IAuthorizationHandler, EmployerViewerAuthorizationHandler>();
        }

        public static void AddSharedAuthenticationServices(this IServiceCollection services)
        {
            services.AddTransient<IEmployerAccountAuthorisationHandler, EmployerAccountAuthorizationHandler>();
            services.AddTransient<IProviderAccountAuthorisationHandler, ProviderAccountAuthorizationHandler>();
            //TODO CHECK IF THESE ARE NEEDED
            //services.AddSingleton<IAuthorizationHandler, ProviderEmployerExternalAccountAuthorizationHandler>();
            //services.AddSingleton<IAuthorizationHandler, AccountActiveAuthorizationHandler>();
            //services.AddSingleton<ITrainingProviderAuthorizationHandler, TrainingProviderAuthorizationHandler>();
            // services.AddSingleton<IAuthorizationHandler, TrainingProviderAllRolesAuthorizationHandler>();
        }
    }
}