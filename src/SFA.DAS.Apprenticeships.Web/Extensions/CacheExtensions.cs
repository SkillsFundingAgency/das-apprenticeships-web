using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace SFA.DAS.Apprenticeships.Web.Extensions
{
	public static class CacheExtensions
	{
		public static async Task<string> SetNewAsync(this IDistributedCache cache, Object obj, int expirationInMinutes = 5)
		{
			var key = Guid.NewGuid().ToString();

			var bytes = ObjectToByteArray(obj);

			await cache.SetAsync(key, bytes, new DistributedCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expirationInMinutes)
			});

			return key;
		}

		public static async Task SetAsync(this IDistributedCache cache, string key, Object obj, int expirationInMinutes = 5)
		{
			var bytes = ObjectToByteArray(obj);

			await cache.SetAsync(key, bytes, new DistributedCacheEntryOptions
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
