using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using SFA.DAS.Apprenticeships.Infrastructure.Configuration;
using SFA.DAS.Apprenticeships.Web.Models;
using System.Text.Json;

namespace SFA.DAS.Apprenticeships.Web.Services
{
	public interface ICacheService
	{
		public Task SetCacheModelAsync(ICacheModel cacheModel);
	}

	public class CacheService : ICacheService
	{
		private readonly IDistributedCache _distributedCache;
		private readonly int _expirationInMinutes;

        public CacheService(IDistributedCache distributedCache, IOptions<CacheConfiguration> options)
        {
			_distributedCache = distributedCache;
			_expirationInMinutes = options.Value.ExpirationInMinutes;
        }

        public async Task SetCacheModelAsync(ICacheModel cacheModel)
		{
			if (string.IsNullOrEmpty(cacheModel.CacheKey))
			{
				cacheModel.CacheKey = Guid.NewGuid().ToString();
			}

			var bytes = ObjectToByteArray(cacheModel);

			await _distributedCache.SetAsync(cacheModel.CacheKey, bytes, new DistributedCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_expirationInMinutes)
			});
		}

		private static byte[] ObjectToByteArray(object obj)
		{
			return JsonSerializer.SerializeToUtf8Bytes(obj);
		}

	}
}
