using System.Diagnostics.CodeAnalysis;
using SFA.DAS.Provider.Shared.UI.Startup;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Web.AppStart;
using SFA.DAS.Provider.Shared.UI.Models;
using SFA.DAS.Apprenticeships.Web.Infrastructure;

namespace SFA.DAS.Apprenticeships.Web
{
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        public static void Main(string[] args)
        {
            //TODO refactor this file!
            //TODO ADD README

            // Logging and initial config
            var builder = WebApplication.CreateBuilder(args);
            var config = builder.Configuration;

            builder.Services.AddApplicationInsightsTelemetry(config["APPINSIGHTS_INSTRUMENTATIONKEY"]);
            builder.ConfigureAzureTableStorage(config);

            //Authentication & Authorization
            var serviceParameters = new ServiceParameters();
            //TODO Store the below info as a claim for use elsewhere in app
            if (config["AuthType"].Equals("Employer", StringComparison.CurrentCultureIgnoreCase))
            {
                serviceParameters.AuthenticationType = AuthenticationType.Employer;
            }
            else if (config["AuthType"].Equals("Provider", StringComparison.CurrentCultureIgnoreCase))
            {
                serviceParameters.AuthenticationType = AuthenticationType.Provider;
            }
            builder.AddConfigurationOptions(config, serviceParameters.AuthenticationType);

            if (serviceParameters.AuthenticationType == AuthenticationType.Employer)
            {
                builder.Services.SetUpEmployerAuthorizationServices();
                builder.Services.SetUpEmployerAuthentication(config, serviceParameters);
            }
            else if (serviceParameters.AuthenticationType == AuthenticationType.Provider)
            {
                builder.Services.AddProviderUiServiceRegistration(config);
                builder.Services.SetUpProviderAuthorizationServices();
                builder.Services.SetUpProviderAuthentication(config);
            }
            builder.Services.AddSharedAuthenticationServices();

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
                //TODO: Figure out what NavigationSection is and whether we need it
                //.SetDefaultNavigationSection(NavigationSection.Home)
                .EnableGoogleAnalytics()
                //TODO: .SetDfESignInConfiguration() [See SFA.DAS.ProviderFunding.Web for example]
                .SetZenDeskConfiguration(config.GetSection("ProviderZenDeskSettings").Get<ZenDeskConfiguration>());
                //TODO: Figure out if/how zen desk config required for employer

            if (!config.IsEnvironmentLocal())
            {
                builder.Services.AddHealthChecks();
            }

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHealthChecks("");

                //TODO Consider some simple exception handling middleware
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
    }
}