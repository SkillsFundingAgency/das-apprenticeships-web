using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Web.Infrastructure;

namespace SFA.DAS.Apprenticeships.Web.Controllers
{
    //TODO: Figure out what NavigationSection is and whether we need it
    //[SetNavigationSection(NavigationSection...)]
    public class ChangeOfPriceController : Controller
    {
        private readonly ILogger<ChangeOfPriceController> _logger;

        public ChangeOfPriceController(ILogger<ChangeOfPriceController> logger)
        {
            _logger = logger;
        }

        [Route("", Name = RouteNames.CreatePriceChangeRequest, Order = 0)]
        public IActionResult CreatePriceChangeRequest()
        {
            return View();
        }
    }
}