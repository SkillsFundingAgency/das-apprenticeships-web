using System.Diagnostics.CodeAnalysis;
using SFA.DAS.Provider.Shared.UI.Startup;

namespace SFA.DAS.Apprenticeships.Web.AppStart
{
    [ExcludeFromCodeCoverage]
    public static class ServiceRegistrationExtensions
    {
        public static void AddCustomServiceRegistration(this IServiceCollection services, IConfiguration config)
        {
            services.AddHttpContextAccessor();
            services.AddProviderUiServiceRegistration(config);
            //TODO: Find equivalent for employer - NuGet already installed for SFA.DAS.Employer.Shared.UI but not visible in there
        }
    }
}