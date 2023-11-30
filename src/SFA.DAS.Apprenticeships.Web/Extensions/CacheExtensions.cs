using Microsoft.Extensions.Caching.Distributed;
using SFA.DAS.Apprenticeships.Web.Models;
using System.Text.Json;

namespace SFA.DAS.Apprenticeships.Web.Extensions
{
	public static class CacheExtensions
	{
		public static async Task SetCacheModelAsync(this IDistributedCache cache, ICacheModel cacheModel, int expirationInMinutes = 5)
		{
			if (string.IsNullOrEmpty(cacheModel.CacheKey))
			{
				cacheModel.CacheKey = Guid.NewGuid().ToString();
			}

			var bytes = ObjectToByteArray(cacheModel);

			await cache.SetAsync(cacheModel.CacheKey, bytes, new DistributedCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expirationInMinutes)
			});
		}

		public static async Task<T?> GetAsync<T>(this IDistributedCache cache, string key)
		{
			var bytes = await cache.GetAsync(key);

			if(bytes == null)
			{
				return default(T);
			}

			return ByteArrayToObject<T>(bytes);
		}

		private static byte[] ObjectToByteArray(Object obj)
		{
			return JsonSerializer.SerializeToUtf8Bytes(obj);
		}

		private static T? ByteArrayToObject<T>(byte[] arrBytes)
		{
			return JsonSerializer.Deserialize<T>(arrBytes);
		}


	}
}
