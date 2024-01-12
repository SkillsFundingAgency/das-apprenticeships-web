
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Web.AppStart;
using SFA.DAS.Apprenticeships.Web.Exceptions;
using SFA.DAS.Apprenticeships.Web.Infrastructure;
using SFA.DAS.Apprenticeships.Web.Middleware;
using SFA.DAS.Apprenticeships.Web.Validators;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.Provider.Shared.UI.Models;
using SFA.DAS.Provider.Shared.UI.Startup;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Apprenticeships.Web
{
	[ExcludeFromCodeCoverage]
    public static class Program
    {
        public static void Main(string[] args)
        {
			try
			{
                TryStartApp(args);
			}
			catch (Exception ex)
			{
				var builder = WebApplication.CreateBuilder(args);
				builder.Services.AddMvc();
				var app = builder.Build();

				if (ex is StartUpException startUpException)
                {
                    FailedStartUpMiddleware.ErrorMessage = $"Failed in startup step: {FailedStartUpMiddleware.StartupStep}: {startUpException.UiSafeMessage}";

				}
                else
                {
					FailedStartUpMiddleware.ErrorMessage = $"Failed in startup step: {FailedStartUpMiddleware.StartupStep}";
				}

				app.UseMiddleware<FailedStartUpMiddleware>();
				app.UseRouting();

				app.Run();
			}
		}

        public static void TryStartApp(string[] args)
        {
			// Logging and initial config
			var builder = WebApplication.CreateBuilder(args);
            var config = builder.Configuration;

			// Logging 
			FailedStartUpMiddleware.StartupStep = "Logging";
			builder.Services.AddApplicationInsightsTelemetry();

			// Config
            builder.ConfigureAzureTableStorage(config);
			config.ValidateConfiguration();
			builder.AddDistributedCache(config);
            builder.AddConfigurationOptions(config);

            // Authentication & Authorization
            var serviceParameters = config.GetServiceParameters();
            switch (serviceParameters.AuthenticationType)
            {
	            case AuthenticationType.Employer:
					FailedStartUpMiddleware.StartupStep = "Employer Authentication";
					Try(() => builder.Services.SetUpEmployerAuthorizationServices(), "SetUpEmployerAuthorizationServices");
					Try(() => builder.Services.SetUpEmployerAuthentication(config, serviceParameters), "SetUpEmployerAuthentication");
					break;
	            case AuthenticationType.Provider:
					FailedStartUpMiddleware.StartupStep = "Provider Authentication";
					Try(() => builder.Services.AddProviderUiServiceRegistration(config), "AddProviderUiServiceRegistration");
					Try(() => builder.Services.SetUpProviderAuthorizationServices(), "SetUpProviderAuthorizationServices");
                    Try(() => builder.Services.SetUpProviderAuthentication(config), "SetUpProviderAuthentication");
                    break;
                default:
					throw new StartUpException("Authentication & Authorization: Invalid authentication type");
            }
            builder.Services.AddAuthorizationPolicies();

            // Configuration of other services and MVC
            builder.Services.AddCustomServiceRegistration(serviceParameters);

			FailedStartUpMiddleware.StartupStep = "Adding MVC builder";
			builder.Services
                .Configure<CookiePolicyOptions>(options =>
                {
                    options.CheckConsentNeeded = context => true;
                    options.MinimumSameSitePolicy = SameSiteMode.None;
                })
                .Configure<IISServerOptions>(options => 
                { 
                    options.AutomaticAuthentication = false; 
                })
                .Configure<RouteOptions>(options => { options.LowercaseUrls = true; })
                .AddSession(options =>
                {
                    options.IdleTimeout = TimeSpan.FromMinutes(10);
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.IsEssential = true;
                })
                .AddMvc(options =>
                {
                    if (!config.IsEnvironmentLocal())
                    {
                        options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                    }
                })
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CreateChangeOfPriceModelValidator>())
                .ConfigureNavigationSection(serviceParameters)
                .EnableGoogleAnalytics()
                .SetDfESignInConfiguration(config.UseDfeSignIn())
                .SetZenDeskConfiguration(config.GetSection("ProviderZenDeskSettings").Get<ZenDeskConfiguration>());

			FailedStartUpMiddleware.StartupStep = "Adding Health Checks";
			if (!config.IsEnvironmentLocal())
            {
                builder.Services.AddHealthChecks();
            }

			FailedStartUpMiddleware.StartupStep = "App Build";
			var app = builder.Build();

            app.AddMiddleware();

			FailedStartUpMiddleware.StartupStep = "Environment Specific app setup";
			if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.CreateHealthCheckEndpoints();
                app.UseExceptionHandler("/Error/500");
                app.UseHsts();
                app.UseContentSecurityPolicy(config);
			}

			FailedStartUpMiddleware.StartupStep = "Closing steps";
			app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.Use(async (context, next) =>
            {
                await EnsurePagesCanBeEmbedded(context, next);
                await Ensure404ResponsesRedirectToErrorPage(context, next);
            });

            app.UseRouting();
            app.MapControllers();
            app.UseAuthentication();
            app.UseAuthorization();

            app.Run();
        }

        private static async Task Ensure404ResponsesRedirectToErrorPage(HttpContext context, Func<Task> next)
        {
            if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
            {
                var originalPath = context.Request.Path.Value;
                context.Items["originalPath"] = originalPath;
                context.Request.Path = "/error/404";
                await next();
            }
        }

        private static async Task EnsurePagesCanBeEmbedded(HttpContext context, Func<Task> next)
        {
            context.Response.Headers["X-Frame-Options"] = "SAMEORIGIN";
            await next();
        }

        private static IMvcBuilder ConfigureNavigationSection(this IMvcBuilder builder, ServiceParameters serviceParameters)
        {
            switch (serviceParameters.AuthenticationType)
            {
                case AuthenticationType.Employer:
                    builder.SetDefaultNavigationSection(Employer.Shared.UI.NavigationSection.ApprenticesHome);
                    break;
                case AuthenticationType.Provider:
                    builder.SetDefaultNavigationSection(Provider.Shared.UI.NavigationSection.ManageApprentices);
                    break;
            }
            return builder;
        }

		public static void Try(Action action, string uiSafeMessage)
		{
			try
			{
				action.Invoke();
			}
			catch(Exception ex)
			{
				if (ex is StartUpException)
				{
					throw;
				}

				throw new StartUpException(uiSafeMessage, ex);
			}
		}
	}
}