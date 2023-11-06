using Microsoft.AspNetCore.Mvc.Infrastructure;
using SFA.DAS.Apprenticeships.Domain.Api;
using SFA.DAS.Apprenticeships.Domain.Interfaces;
using System.Diagnostics.CodeAnalysis;
using SFA.DAS.Apprenticeships.Application.Services;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.Apprenticeships.Web.AppStart
{
    [ExcludeFromCodeCoverage]
    public static class ServiceRegistrationExtensions
    {
        public static void AddCustomServiceRegistration(
            this IServiceCollection services, 
            ServiceParameters serviceParameters)
        {
            services.AddSingleton(serviceParameters);
            services.AddHttpContextAccessor();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddHttpClient<IApiClient, ApiClient>();
            services.AddTransient<IEmployerAccountService, EmployerAccountService>();
            services.AddTransient<ICustomClaims, EmployerAccountPostAuthenticationClaimsHandler>();
        }
    }
}