﻿using SFA.DAS.Apprenticeships.Web.Exceptions;
using SFA.DAS.Apprenticeships.Web.Infrastructure;
using SFA.DAS.Apprenticeships.Web.Middleware;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using ConfigurationManager = Microsoft.Extensions.Configuration.ConfigurationManager;

namespace SFA.DAS.Apprenticeships.Web.AppStart
{
	[ExcludeFromCodeCoverage]
	public static class ConfigurationManagerExtensions
    {
        public static ServiceParameters GetServiceParameters(this ConfigurationManager config)
        {
			FailedStartUpMiddleware.StartupStep = "GetServiceParameters";
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

        public static void ValidateConfiguration(this ConfigurationManager config)
        {
			FailedStartUpMiddleware.StartupStep = "ValidateConfiguration";
            if (!config.HasConfigValue("ResourceEnvironmentName"))
            {
                throw new StartUpException("ResourceEnvironmentName is not set in configuration.");
            }
		}
    }
}
