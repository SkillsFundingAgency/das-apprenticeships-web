using SFA.DAS.Apprenticeships.Web.Middleware;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Apprenticeships.Web.AppStart;

[ExcludeFromCodeCoverage]
public static class MiddlewareExtensions
{
    public static void AddMiddleware(this WebApplication webApplication)
    {
        FailedStartUpMiddleware.StartupStep = "AddMiddleware";
        webApplication.UseMiddleware<CacheMiddleware>();		
    }
}