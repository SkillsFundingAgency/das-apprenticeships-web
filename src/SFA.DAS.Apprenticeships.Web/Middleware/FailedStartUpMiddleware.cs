namespace SFA.DAS.Apprenticeships.Web.Middleware
{ 
	/// <summary>
	/// This middleware returns a static error message when the application fails to start
	/// </summary>
	public class FailedStartUpMiddleware
	{
		internal static string StartupStep = "Initialized";
		internal static string ErrorMessage = "App Failed to start";

		private readonly RequestDelegate _next;

		public FailedStartUpMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			await _next(context);
			await context.Response.WriteAsync(ErrorMessage);
		}
	}
}
