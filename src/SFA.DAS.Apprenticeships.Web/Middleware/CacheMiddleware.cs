using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Primitives;
using System.Text.Json;

namespace SFA.DAS.Apprenticeships.Web.Middleware
{
	/// <summary>
	/// The purpose of this middleware is to retreive data from the cache and add it to the request
	/// </summary>
	public class CacheMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly IDistributedCache _distributedCache;

		public CacheMiddleware(RequestDelegate next, IDistributedCache distributedCache)
		{
			_next = next;
			_distributedCache = distributedCache;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			var cacheKey = GetCacheKey(context);
			if(!string.IsNullOrEmpty(cacheKey))
			{
				await PopulateRequestFromCache(context, cacheKey);
			}

			await _next(context);

		}

		private async Task PopulateRequestFromCache(HttpContext context, string cacheKey)
		{
			var json = await _distributedCache.GetStringAsync(cacheKey);
			if (string.IsNullOrEmpty(json))
				return;

			var cachedValues = GetFlatJson(json);

			if (cachedValues == null)
				return;

			var newForm = GetForm(context);

			// Add new form values
			foreach (var key in cachedValues.Keys)
			{
				if (!newForm.ContainsKey(key))
				{
					newForm.Add(key, cachedValues[key].ToString());
				}
			}

			context.Request.Form = new FormCollection(newForm);
		}

		private static string? GetCacheKey(HttpContext context)
		{
			var cacheKey = context.Request.Query["cacheKey"].ToString();

			if (!string.IsNullOrEmpty(cacheKey))
				return cacheKey;

			if (context.Request.HasFormContentType && context.Request.Form.ContainsKey("cacheKey"))
				return context.Request.Form["cacheKey"].ToString();

			return null;
		}	

		private static Dictionary<string, StringValues> GetForm(HttpContext context)
		{
			var newForm = new Dictionary<string, StringValues>();
			if (!context.Request.HasFormContentType)
				return newForm;

			var existingForm = context.Request.Form;

			// Add existing form values
			foreach (var key in existingForm.Keys)
			{
				existingForm.TryGetValue(key, out StringValues formValues);
				newForm.Add(key, formValues);
			}

			return newForm;
		}

		public static Dictionary<string, JsonElement> GetFlatJson(string json)
		{
			IEnumerable<(string Path, JsonProperty P)> GetLeaves(string path, JsonProperty p)
				=> p.Value.ValueKind != JsonValueKind.Object
					? new[] { (Path: path == null ? p.Name : path + "." + p.Name, p) }
					: p.Value.EnumerateObject().SelectMany(child => GetLeaves(path == null ? p.Name : path + "." + p.Name, child));

			using (JsonDocument document = JsonDocument.Parse(json)) // Optional JsonDocumentOptions options
				return document.RootElement.EnumerateObject()
					.SelectMany(p => GetLeaves(null, p))
					.ToDictionary(k => k.Path, v => v.P.Value.Clone()); //Clone so that we can use the values outside of using
		}
	}
}
