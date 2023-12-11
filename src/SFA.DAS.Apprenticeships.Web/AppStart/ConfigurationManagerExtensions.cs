using SFA.DAS.Apprenticeships.Web.Infrastructure;
using System.Configuration;
using ConfigurationManager = Microsoft.Extensions.Configuration.ConfigurationManager;

namespace SFA.DAS.Apprenticeships.Web.AppStart
{
    public static class ConfigurationManagerExtensions
    {
        public static ServiceParameters GetServiceParameters(this ConfigurationManager config)
        {
            var serviceParameters = new ServiceParameters();
            if (config.IsConfigValue("ApprenticeshipsWeb:AuthType", "Employer"))
            {
                serviceParameters.AuthenticationType = AuthenticationType.Employer;
            }
            else if (config.IsConfigValue("ApprenticeshipsWeb:AuthType", "Provider"))
            {
                serviceParameters.AuthenticationType = AuthenticationType.Provider;
            }
            else
            {
                throw new ConfigurationErrorsException($"Configuration for a valid 'ApprenticeshipsWeb:AuthType' not found.");
            }

            return serviceParameters;
        }
    }
}
