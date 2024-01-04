using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Web.AppStart;
using SFA.DAS.Apprenticeships.Web.Infrastructure;
using SFA.DAS.Apprenticeships.Web.Validators;
using SFA.DAS.Provider.Shared.UI.Models;
using SFA.DAS.Provider.Shared.UI.Startup;
using System.Diagnostics.CodeAnalysis;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.Apprenticeships.Infrastructure.Configuration;

namespace SFA.DAS.Apprenticeships.Web
{
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        public static void Main(string[] args)
        {
            // Logging and initial config
            var builder = WebApplication.CreateBuilder(args);
            var config = builder.Configuration;

            // Logging & caching
            builder.Services.AddApplicationInsightsTelemetry();
            builder.AddDistributedCache(config);

            // Config
            builder.ConfigureAzureTableStorage(config);            
            builder.AddDistributedCache(config);
            builder.AddConfigurationOptions(config);

            // Authentication & Authorization
            var serviceParameters = config.GetServiceParameters();
            switch (serviceParameters.AuthenticationType)
            {
	            case AuthenticationType.Employer:
		            builder.Services.SetUpEmployerAuthorizationServices();
		            builder.Services.SetUpEmployerAuthentication(config, serviceParameters);
		            break;
	            case AuthenticationType.Provider:
		            builder.Services.AddProviderUiServiceRegistration(config);
		            builder.Services.SetUpProviderAuthorizationServices();
		            builder.Services.SetUpProviderAuthentication(config);
		            break;
            }
            builder.Services.AddAuthorizationPolicies();

            // Configuration of other services and MVC
            builder.Services.AddCustomServiceRegistration(serviceParameters);
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

            if (!config.IsEnvironmentLocal())
            {
                builder.Services.AddHealthChecks();
            }

            var app = builder.Build();

            app.AddMiddleware();

			if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.CreateHealthCheckEndpoints();
                app.UseExceptionHandler("/Error/500");
                app.UseHsts();
            }

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
    }
}