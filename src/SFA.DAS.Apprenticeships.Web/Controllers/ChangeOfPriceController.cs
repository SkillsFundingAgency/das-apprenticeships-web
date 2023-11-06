using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Domain.Interfaces;
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
        public const string ChangeOfPriceRequestViewName = "CreatePriceChangeRequest";

        public ChangeOfPriceController(ILogger<ChangeOfPriceController> logger, IApprenticeshipService apprenticeshipService, IMapper<CreateChangeOfPriceModel> mapper)
        {
            _logger = logger;
            _apprenticeshipService = apprenticeshipService;
            _mapper = mapper;
        }

        [Route("ChangeOfPrice/{apprenticeshipId}", Name = RouteNames.CreatePriceChangeRequest, Order = 0)]
        public async Task<IActionResult> GetPage(string apprenticeshipId)
        {
            var apprenticeshipPrice = await _apprenticeshipService.GetApprenticeshipPrice(apprenticeshipId);
            var model = _mapper.Map(apprenticeshipPrice);
            return View(ChangeOfPriceRequestViewName, model);
        }

        [HttpPost]
        [Route("ChangeOfPrice/{apprenticeshipId}")]
        public IActionResult CreatePriceChangeRequest(CreateChangeOfPriceModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(ChangeOfPriceRequestViewName, model);
            }

            throw new NotImplementedException("Actions here to be completed in later User Story");
        }
    }
}