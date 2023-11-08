using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Web.Infrastructure;

namespace SFA.DAS.Apprenticeships.Web.Controllers
{
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