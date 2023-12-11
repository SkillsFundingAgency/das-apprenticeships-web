using SFA.DAS.Apprenticeships.Infrastructure.Configuration;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Caching.StackExchangeRedis;

namespace SFA.DAS.Apprenticeships.Web.AppStart
{
	[ExcludeFromCodeCoverage]
	public static class AddDistributedCacheExtension
	{
		public static void AddDistributedCache(this WebApplicationBuilder builder, ConfigurationManager config)
		{
#if DEBUG
			builder.Services.AddDistributedMemoryCache();
#else
			AddRedisCache(builder, config);
#endif
		}

		private static void AddRedisCache(WebApplicationBuilder builder, ConfigurationManager config)
		{
			var cacheConfiguration = config.GetSection(nameof(CacheConfiguration)).Get<CacheConfiguration>();
			
			builder.Services.AddOptions<RedisCacheOptions>().Configure<IServiceProvider>((options, serviceProvider) =>
			{
				options.Configuration = cacheConfiguration.CacheConnection;
			});

			builder.Services.AddStackExchangeRedisCache(options =>
			{
				options.Configuration = cacheConfiguration.CacheConnection;
				options.InstanceName = cacheConfiguration.DefaultCache;
			});
		}
	}
}
