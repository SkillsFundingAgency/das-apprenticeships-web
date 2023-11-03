using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Web.Infrastructure;
using SFA.DAS.Apprenticeships.Web.Models;

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
            var model = TempPopulateModel();
            return View(model);
        }

        [HttpPost]
        [Route("")]
        public IActionResult CreatePriceChangeRequest(CreateChangeOfPriceModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //  Actions here to be completed in later User Story
            return View(model);
        }

        private CreateChangeOfPriceModel TempPopulateModel()
        {
            return new CreateChangeOfPriceModel
            {
                FundingBandMaximum = 9000,
                ApprenticeshipTrainingPrice = 6000,
                ApprenticeshipEndPointAssessmentPrice = 2000,
            };
        }
    }
}