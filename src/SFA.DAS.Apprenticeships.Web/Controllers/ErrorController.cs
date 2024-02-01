using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Domain.Api;
using SFA.DAS.Apprenticeships.Web.Infrastructure;
using SFA.DAS.Apprenticeships.Web.Models.Error;
using SFA.DAS.Provider.Shared.UI.Attributes;

namespace SFA.DAS.Apprenticeships.Web.Controllers
{
    [HideNavigationBar(hideAccountHeader: false, hideNavigationLinks: true)]
    [Route("[controller]")]
    public class ErrorController : Controller
    {
        private readonly IConfiguration _configuration;

        public ErrorController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Route("{statuscode?}")]
        public IActionResult Error(int? statusCode)
        {
            if (HttpContext?.Features?.Get<IExceptionHandlerFeature>()?.Error is ApiUnauthorizedException) return View("401");

            switch (statusCode)
            {
                case 403:
                    var useDfESignIn = _configuration.UseDfeSignIn();
                    var dashboardLink = _configuration["ProviderSharedUIConfiguration:DashboardUrl"];
                    var testPrefixForHelpPageLink = !_configuration.IsConfigValue("ResourceEnvironmentName","prd") ? "test-" : "";
                    var helpPageLink =
                        $"https://{testPrefixForHelpPageLink}services.signin.education.gov.uk/approvals/select-organisation?action=request-service";
                    return View(statusCode.ToString(), new Error403ViewModel(dashboardLink, helpPageLink, useDfESignIn));
                case 404:
                case 401:
                    return View(statusCode.ToString());
                default:
                    return View();
            }
        }
    }
}