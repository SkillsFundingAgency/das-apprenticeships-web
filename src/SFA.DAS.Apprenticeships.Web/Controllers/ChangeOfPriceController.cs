using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Domain.Interfaces;
using SFA.DAS.Apprenticeships.Web.Extensions;
using SFA.DAS.Apprenticeships.Web.Infrastructure;
using SFA.DAS.Apprenticeships.Web.Models;

namespace SFA.DAS.Apprenticeships.Web.Controllers
{
    //TODO: Figure out what NavigationSection is and whether we need it
    //[SetNavigationSection(NavigationSection...)]
    public class ChangeOfPriceController : Controller
    {
        private readonly ILogger<ChangeOfPriceController> _logger;
        private readonly IApprenticeshipService _apprenticeshipService;
        private readonly IMapper<CreateChangeOfPriceModel> _mapper;
        public const string ProviderInitiatedViewName = "ProviderInitiated";

        public ChangeOfPriceController(ILogger<ChangeOfPriceController> logger, IApprenticeshipService apprenticeshipService, IMapper<CreateChangeOfPriceModel> mapper)
        {
            _logger = logger;
            _apprenticeshipService = apprenticeshipService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("provider/{ukprn}/ChangeOfPrice/{apprenticeshipHashedId}")]
        public async Task<IActionResult> GetProviderInitiatedPage(string apprenticeshipHashedId)
        {
            var apprenticeshipPrice = await _apprenticeshipService.GetApprenticeshipPrice(apprenticeshipHashedId);
            var model = _mapper.Map(apprenticeshipPrice);
            PopulateProviderInitiatedRouteValues(model);
            return View(ProviderInitiatedViewName, model);
        }

        [HttpPost]
        [Route("provider/{ukprn}/ChangeOfPrice/{apprenticeshipHashedId}")]
        public IActionResult ProviderInitiatedPriceChangeRequest(CreateChangeOfPriceModel model)
        {
            if (!ModelState.IsValid)
            {
                PopulateProviderInitiatedRouteValues(model);
                return View(ProviderInitiatedViewName, model);
            }

            throw new NotImplementedException("Actions here to be completed in later User Story");
        }

        //  If other endpoints use the same route values, this could be refactored to take an interface/abstract class instead of CreateChangeOfPriceModel
        private void PopulateProviderInitiatedRouteValues(CreateChangeOfPriceModel model)
        {
            model.ApprenticeshipHashedId = HttpContext.GetRouteValueString(RouteValues.ApprenticeshipHashedId);
            model.ProviderReferenceNumber = HttpContext.GetRouteValueString(RouteValues.Ukprn);
        }
    }
}