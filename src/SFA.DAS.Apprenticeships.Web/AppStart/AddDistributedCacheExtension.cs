using SFA.DAS.Apprenticeships.Infrastructure.Configuration;
using System.Diagnostics.CodeAnalysis;

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

			builder.Services.AddStackExchangeRedisCache(options =>
			{
				options.Configuration = cacheConfiguration.CacheConnection;
				options.InstanceName = cacheConfiguration.DefaultCache;
			});
		}
	}
}
