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
                    var testPrefixForHelpPageLink = !_configuration.IsConfigValue("ResourceEnvironmentName","prd") ? "test-" : "";
                    return View(statusCode.ToString(), new Error403ViewModel()
                    {
                        UseDfESignIn = _configuration.UseDfeSignIn(),
                        HelpPageLink = $"https://{testPrefixForHelpPageLink}services.signin.education.gov.uk/approvals/select-organisation?action=request-service"
                        
                        //TODO: Dashboard link will need to be conditional on whether employer or provider is authenticated
                        //DashboardLink = _configuration["ProviderSharedUIConfiguration:DashboardUrl"]
                    });
                case 404:
                    return View(statusCode.ToString());
                default:
                    return View();
            }
        }
    }
}
