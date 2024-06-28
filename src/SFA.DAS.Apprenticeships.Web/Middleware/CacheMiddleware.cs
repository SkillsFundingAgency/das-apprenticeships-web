using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Primitives;
using SFA.DAS.Apprenticeships.Web.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Apprenticeships.Web.Middleware;

/// <summary>
/// The purpose of this middleware is to retreive data from the cache and add it to the request
/// </summary>
[ExcludeFromCodeCoverage]
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

        var cachedValues = json.GetFlatJson();

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

}