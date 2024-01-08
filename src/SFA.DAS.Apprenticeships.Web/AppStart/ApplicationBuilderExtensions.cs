namespace SFA.DAS.Apprenticeships.Web.AppStart;

public static class ApplicationBuilderExtensions
{
	public static IApplicationBuilder UseContentSecurityPolicy(this IApplicationBuilder app)
	{
		app.Use(async (context, next) =>
		{
			context.Response.Headers["Content-Security-Policy"] =
				"default-src 'self' 'unsafe-inline' https://*.zdassets.com https://*.zendesk.com wss://*.zendesk.com wss://*.zopim.com *.google-analytics.com; " +
				"style-src 'self' 'unsafe-inline' https://*.azureedge.net; " +
				"img-src 'self' https://*.azureedge.net *.google-analytics.com https://*.zdassets.com https://*.zendesk.com wss://*.zendesk.com wss://*.zopim.com; " +
				"script-src 'self' 'unsafe-inline' das-at-frnt-end.azureedge.net " +
				"*.googletagmanager.com *.google-analytics.com *.googleapis.com https://*.zdassets.com https://*.zendesk.com wss://*.zendesk.com wss://*.zopim.com; " +
				"font-src 'self' das-at-frnt-end.azureedge.net data:;";
			await next();

		});

		return app;
	}
}