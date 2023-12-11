using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Apprenticeships.Web.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public static class ConfigurationExtension
    {
        public static bool IsConfigValue(this IConfiguration config, string key, string value)
        {
            return !string.IsNullOrEmpty(config[key]) && config[key].Equals(value, StringComparison.CurrentCultureIgnoreCase);
        }
        public static bool IsEnvironmentLocal(this IConfiguration configuration)
        {
            return configuration["ResourceEnvironmentName"].StartsWith("LOCAL", StringComparison.CurrentCultureIgnoreCase);
        }

        public static bool UseLocalStubAuth(this IConfiguration config)
        {
            return IsTrueInConfig("ApprenticeshipsWeb:StubAuth", config);
        }

        public static bool UseGovSignIn(this IConfiguration config)
        {
            return IsTrueInConfig("ApprenticeshipsWeb:UseGovSignIn", config);
        }

        public static bool UseDfeSignIn(this IConfiguration config)
        {
            return IsTrueInConfig("ApprenticeshipsWeb:UseDfESignIn", config);
        }

        private static bool IsTrueInConfig(string key, IConfiguration config)
        {
            return config[key] != null && config[key].Equals("true", StringComparison.CurrentCultureIgnoreCase);
        }
    }
}