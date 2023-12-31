using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;
using SFA.DAS.Apprenticeships.Web.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;

namespace SFA.DAS.Apprenticeships.Web.AppStart
{
    [ExcludeFromCodeCoverage]
    public static class ConfigurationExtensions
    {
        public static void AddConfigurationOptions(this WebApplicationBuilder builder, ConfigurationManager config)
        {
            builder.Services.AddOptions()
                .Configure<ApprenticeshipsOuterApiConfiguration>(config.GetSection("ApprenticeshipsOuterApiConfiguration"));
            builder.Services.AddSingleton(cfg => cfg.GetService<IOptions<ApprenticeshipsOuterApiConfiguration>>()!.Value);
        }

        public static void ConfigureAzureTableStorage(this WebApplicationBuilder builder, ConfigurationManager config)
        {
            config.AddAzureTableStorage(options =>
            {
                var (names, connectionString, environment) = builder.BaseConfigurationValues();
                options.ConfigurationKeys = names.Split(",");
                options.StorageConnectionString = connectionString;
                options.EnvironmentName = environment;
                options.PreFixConfigurationKeys = false;
            });
        }
        public static bool IsLocal(this IConfiguration configuration)
        {
            return configuration["EnvironmentName"].StartsWith("LOCAL", StringComparison.CurrentCultureIgnoreCase);
        }

        private static (string names, string connectionString, string environment) BaseConfigurationValues(this WebApplicationBuilder configBuilder)
        {
            var config = configBuilder.Configuration;
            return
            (
                config["ConfigNames"],
                config["ConfigurationStorageConnectionString"],
                config["EnvironmentName"]
            );
        }
    }
}