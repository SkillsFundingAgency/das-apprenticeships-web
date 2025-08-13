using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Apprenticeships.Web.AppStart;

[ExcludeFromCodeCoverage]
public static class ApplicationBuilderExtensions
{
	public static IApplicationBuilder UseContentSecurityPolicy(this IApplicationBuilder app, ConfigurationManager config)
	{
		var cdn = config.GetSection("Cdn:Url").Get<string>();
		app.Use(async (context, next) =>
		{
			context.Response.Headers["Content-Security-Policy"] =
                "default-src 'self' 'unsafe-inline' https://*.zdassets.com https://*.zendesk.com wss://*.zendesk.com wss://*.zopim.com *.google-analytics.com https://*.rcrsv.io; " +
                "style-src 'self' 'unsafe-inline' https://*.azureedge.net https://*.rcrsv.io; " +
                "img-src 'self' https://*.azureedge.net *.google-analytics.com https://*.zdassets.com https://*.zendesk.com wss://*.zendesk.com wss://*.zopim.com https://*.rcrsv.io; " +
				$"script-src 'self' 'unsafe-inline' {cdn} " +
                "*.googletagmanager.com *.google-analytics.com *.googleapis.com https://*.zdassets.com https://*.zendesk.com wss://*.zendesk.com wss://*.zopim.com https://*.rcrsv.io; " +
				$"font-src 'self' {cdn} https://*.rcrsv.io data:;";
			await next();

		});

		return app;
	}
}