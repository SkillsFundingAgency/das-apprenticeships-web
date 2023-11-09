using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
                    return View(statusCode.ToString(), new Error403ViewModel(_configuration["ResourceEnvironmentName"])
                    {
                        //UseDfESignIn = _configuration["UseDfESignIn"] != null && _configuration["UseDfESignIn"].Equals("true", StringComparison.CurrentCultureIgnoreCase),
                        
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
