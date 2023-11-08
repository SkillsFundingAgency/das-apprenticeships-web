using System.Diagnostics.CodeAnalysis;
using SFA.DAS.Provider.Shared.UI.Startup;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Web.AppStart;
using SFA.DAS.Provider.Shared.UI.Models;
using SFA.DAS.Provider.Shared.UI;

namespace SFA.DAS.Apprenticeships.Web
{
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var config = builder.Configuration;

            // Logging
            builder.Services.AddApplicationInsightsTelemetry();

            // Config
            builder.ConfigureAzureTableStorage(config);
            builder.AddConfigurationOptions(config);

            // Configure services and MVC
            builder.Services.AddCustomServiceRegistration(config);
            builder.Services
                .Configure<CookiePolicyOptions>(options =>
                {
                    options.CheckConsentNeeded = context => true;
                    options.MinimumSameSitePolicy = SameSiteMode.None;
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
                    if (!config.IsLocal())
                    {
                        options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                    }
                })
                //.SetDefaultNavigationSection(NavigationSection.Home)
                .EnableGoogleAnalytics()
                .SetZenDeskConfiguration(config.GetSection("ProviderZenDeskSettings").Get<ZenDeskConfiguration>());

            if (!config.IsLocal())
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