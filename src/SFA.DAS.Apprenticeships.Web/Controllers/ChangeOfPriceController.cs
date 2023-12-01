using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using SFA.DAS.Apprenticeships.Domain.Interfaces;
using SFA.DAS.Apprenticeships.Web.Extensions;
using SFA.DAS.Apprenticeships.Web.Infrastructure;
using SFA.DAS.Apprenticeships.Web.Models;
using SFA.DAS.Apprenticeships.Web.Services;
using SFA.DAS.Provider.Shared.UI;
using SFA.DAS.Provider.Shared.UI.Attributes;

namespace SFA.DAS.Apprenticeships.Web.Controllers
{
	public class ChangeOfPriceController : Controller
    {
        private readonly ILogger<ChangeOfPriceController> _logger;
        private readonly IApprenticeshipService _apprenticeshipService;
        private readonly IMapper<CreateChangeOfPriceModel> _mapper;
        private readonly ICacheService _cache;
        public const string ProviderInitiatedViewName = "ProviderInitiated";
        public const string ProviderInitiatedCheckDetailsViewName = "ProviderInitiatedCheckDetails";

        public ChangeOfPriceController(
            ILogger<ChangeOfPriceController> logger, 
            IApprenticeshipService apprenticeshipService, 
            IMapper<CreateChangeOfPriceModel> mapper,
			ICacheService cache)
        {
            _logger = logger;
            _apprenticeshipService = apprenticeshipService;
            _mapper = mapper;
			_cache = cache;

		}

        [HttpGet]
        [SetNavigationSection(NavigationSection.ManageApprentices)]
        [Route("provider/{ukprn}/ChangeOfPrice/{apprenticeshipHashedId}")]
        public async Task<IActionResult> GetProviderInitiatedPage(string apprenticeshipHashedId)
        {
            var apprenticeshipKey = await _apprenticeshipService.GetApprenticeshipKey(apprenticeshipHashedId);
            if(apprenticeshipKey == default(Guid))
            {
                _logger.LogWarning($"Apprenticeship key not found for apprenticeship with hashed id {apprenticeshipHashedId}");
                return NotFound();
            }

            var apprenticeshipPrice = await _apprenticeshipService.GetApprenticeshipPrice(apprenticeshipKey);
            if (apprenticeshipPrice == null || apprenticeshipPrice.ApprenticeshipKey != apprenticeshipKey)
            {
                _logger.LogWarning($"ApprenticeshipPrice not found for apprenticeshipKey {apprenticeshipKey}");
                return NotFound();
            }

            var model = _mapper.Map(apprenticeshipPrice);
            PopulateProviderInitiatedRouteValues(model);
            await _cache.SetCacheModelAsync(model);
            return View(ProviderInitiatedViewName, model);
        }

        [HttpGet]
        [Route("provider/{ukprn}/ChangeOfPrice/{apprenticeshipHashedId}/edit")]
        public IActionResult GetProviderInitiatedEditPage(CreateChangeOfPriceModel model)
        {
            return View(ProviderInitiatedViewName, model);
        }

        [HttpPost]
        [SetNavigationSection(NavigationSection.ManageApprentices)]
        [Route("provider/{ukprn}/ChangeOfPrice/{apprenticeshipHashedId}")]
        public async Task<IActionResult> ProviderInitiatedCheckDetailsPage(CreateChangeOfPriceModel model)
        {
			PopulateProviderInitiatedRouteValues(model);
			if (!ModelState.IsValid)
            {
                return View(ProviderInitiatedViewName, model);
            }

            await _cache.SetCacheModelAsync(model);
			return View(ProviderInitiatedCheckDetailsViewName, model);
        }

		[HttpPost]
		[Route("provider/{ukprn}/ChangeOfPrice/{apprenticeshipHashedId}/submit")]
		public IActionResult ProviderInitiatedSubmitChange(CreateChangeOfPriceModel model)
		{
			return View(ProviderInitiatedCheckDetailsViewName, model); // For now return to previous page, functionality to be added later
		}

		//  If other endpoints use the same route values, this could be refactored to take an interface/abstract class instead of CreateChangeOfPriceModel
		private void PopulateProviderInitiatedRouteValues(CreateChangeOfPriceModel model)
        {
            model.ApprenticeshipHashedId = HttpContext.GetRouteValueString(RouteValues.ApprenticeshipHashedId);
            model.ProviderReferenceNumber = HttpContext.GetRouteValueString(RouteValues.Ukprn);
        }
    }
}