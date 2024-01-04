using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;
using SFA.DAS.Apprenticeships.Infrastructure.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;

namespace SFA.DAS.Apprenticeships.Web.AppStart
{
    [ExcludeFromCodeCoverage]
    public static class AddConfigurationExtension
    {
        public static void AddConfigurationOptions(
            this WebApplicationBuilder builder, 
            ConfigurationManager config)
        {
            builder.Services.Configure<ApprenticeshipsWeb>(config.GetSection(nameof(ApprenticeshipsWeb)));
            builder.Services.AddSingleton(cfg => cfg.GetService<IOptions<ApprenticeshipsWeb>>()!.Value);

            builder.Services.Configure<ApprenticeshipsOuterApi>(config.GetSection(nameof(ApprenticeshipsOuterApi)));
            builder.Services.AddSingleton(cfg => cfg.GetService<IOptions<ApprenticeshipsOuterApi>>()!.Value);

			builder.Services.Configure<CacheConfiguration>(config.GetSection(nameof(CacheConfiguration)));
			builder.Services.AddSingleton(cfg => cfg.GetService<IOptions<CacheConfiguration>>()!.Value);
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