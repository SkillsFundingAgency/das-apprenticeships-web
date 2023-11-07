using System.Configuration;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Web.AppStart;
using SFA.DAS.Apprenticeships.Web.Infrastructure;
using SFA.DAS.Apprenticeships.Web.Validators;
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
            //TODO ADD README

            // Logging and initial config
            var builder = WebApplication.CreateBuilder(args);
            var config = builder.Configuration;

            // Logging
            builder.Services.AddApplicationInsightsTelemetry();

            // Config
            builder.ConfigureAzureTableStorage(config);
            builder.AddDistributedCache(config);

            //Authentication & Authorization
            var serviceParameters = new ServiceParameters();
            //TODO Store the below info as a claim for use elsewhere in app
            if (config.IsConfigValue("AuthType", "Employer"))
            {
                serviceParameters.AuthenticationType = AuthenticationType.Employer;
            }
            else if (config.IsConfigValue("AuthType", "Provider"))
            {
                serviceParameters.AuthenticationType = AuthenticationType.Provider;
            }
            else
            {
                throw new ConfigurationErrorsException($"Configuration for a valid 'AuthType' not found.");
            }
            builder.AddConfigurationOptions(config, serviceParameters.AuthenticationType);

            if (serviceParameters.AuthenticationType == AuthenticationType.Employer)
            {
                //builder.Services.SetUpEmployerAuthorizationServices();
                //builder.Services.SetUpEmployerAuthentication(config, serviceParameters);
            }
            else if (serviceParameters.AuthenticationType == AuthenticationType.Provider)
            {
                builder.Services.AddProviderUiServiceRegistration(config);
                //builder.Services.SetUpProviderAuthorizationServices();
                //builder.Services.SetUpProviderAuthentication(config);
            }
            //builder.Services.AddSharedAuthenticationServices();

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
                .SetDefaultNavigationSection(Provider.Shared.UI.NavigationSection.ManageApprentices)
                //.SetDefaultNavigationSection(Employer.Shared.UI.NavigationSection.ApprenticesHome)
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
    }
}