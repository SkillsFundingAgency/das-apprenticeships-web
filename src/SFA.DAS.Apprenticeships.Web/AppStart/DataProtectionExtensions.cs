using Microsoft.AspNetCore.DataProtection;
using SFA.DAS.Apprenticeships.Infrastructure.Configuration;
using StackExchange.Redis;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Apprenticeships.Web.AppStart;

[ExcludeFromCodeCoverage]
public static class DataProtectionExtensions
{
	public static IServiceCollection AddDataProtection(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
	{
		if (!environment.IsDevelopment())
		{
			var dataProtectionConfig = configuration.GetSection(nameof(DataProtection))
				.Get<DataProtection>();

			if (dataProtectionConfig != null)
			{
				var redisConnectionString = dataProtectionConfig.RedisConnectionString;
				var dataProtectionKeysDatabase = dataProtectionConfig.DataProtectionKeysDatabase;

				var redis = ConnectionMultiplexer
					.Connect($"{redisConnectionString},{dataProtectionKeysDatabase}");

				services.AddDataProtection()
					.SetApplicationName("das-employer")
					.PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");
			}
		}
		return services;
	}
}
