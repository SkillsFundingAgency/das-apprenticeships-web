using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Web.Infrastructure;
using SFA.DAS.Apprenticeships.Web.Models.Error;

namespace SFA.DAS.Apprenticeships.Web.Controllers
{
    [AllowAnonymous]
    [Route("[controller]")]
    public class ErrorController : Controller
    {
        private readonly IConfiguration _configuration;

        public ErrorController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Route("error/{statuscode?}")]
        public IActionResult Error(int? statusCode)
        {
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
                    return View(statusCode.ToString());
                default:
                    return View();
            }
        }
    }
}
