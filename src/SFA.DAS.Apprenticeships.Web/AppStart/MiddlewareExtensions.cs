using SFA.DAS.Apprenticeships.Web.Middleware;

namespace SFA.DAS.Apprenticeships.Web.AppStart
{
	public static class MiddlewareExtensions
	{
		public static void AddMiddleware(this WebApplication webApplication)
		{
			webApplication.UseMiddleware<CacheMiddleware>();		
		}
	}
}
