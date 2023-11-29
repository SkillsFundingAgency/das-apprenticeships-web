using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using SFA.DAS.Apprenticeships.Domain.Interfaces;
using SFA.DAS.Apprenticeships.Web.Extensions;
using SFA.DAS.Apprenticeships.Web.Infrastructure;
using SFA.DAS.Apprenticeships.Web.Models;
using System.Text.Json;

namespace SFA.DAS.Apprenticeships.Web.Controllers
{
	public class ChangeOfPriceController : Controller
    {
        private readonly ILogger<ChangeOfPriceController> _logger;
        private readonly IApprenticeshipService _apprenticeshipService;
        private readonly IMapper<CreateChangeOfPriceModel> _mapper;
        private readonly IDistributedCache _distributedCache;
        public const string ProviderInitiatedViewName = "ProviderInitiated";
        public const string ProviderInitiatedCheckDetails = "ProviderInitiatedCheckDetails";

        public ChangeOfPriceController(
            ILogger<ChangeOfPriceController> logger, 
            IApprenticeshipService apprenticeshipService, 
            IMapper<CreateChangeOfPriceModel> mapper,
            IDistributedCache distributedCache)
        {
            _logger = logger;
            _apprenticeshipService = apprenticeshipService;
            _mapper = mapper;
			_distributedCache = distributedCache;

		}

        [HttpGet]
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
            model.CacheKey = await _distributedCache.SetNewAsync(model);
            return View(ProviderInitiatedViewName, model);
        }

		[HttpGet]
		[Route("provider/{ukprn}/ChangeOfPrice/{apprenticeshipHashedId}/edit/{cacheKey}")]
		public async Task<IActionResult> GetProviderInitiatedEditPage(string cacheKey)
		{
            var model = await _distributedCache.GetAsync<CreateChangeOfPriceModel>(cacheKey);
			if (model == null)
			{
				_logger.LogWarning($"CreateChangeOfPriceModel could not be found for cacheKey {cacheKey}");
				return NotFound();
			}

			return View(ProviderInitiatedViewName, model);
		}

		[HttpPost]
        [Route("provider/{ukprn}/ChangeOfPrice/{apprenticeshipHashedId}")]
        public async Task<IActionResult> ProviderInitiatedPriceChangeRequest(CreateChangeOfPriceModel model)
        {
			PopulateProviderInitiatedRouteValues(model);
			if (!ModelState.IsValid)
            {
                return View(ProviderInitiatedViewName, model);
            }

            await _distributedCache.SetAsync(model.CacheKey!, model);
			return View(ProviderInitiatedCheckDetails, model);
        }

        //  If other endpoints use the same route values, this could be refactored to take an interface/abstract class instead of CreateChangeOfPriceModel
        private void PopulateProviderInitiatedRouteValues(CreateChangeOfPriceModel model)
        {
            model.ApprenticeshipHashedId = HttpContext.GetRouteValueString(RouteValues.ApprenticeshipHashedId);
            model.ProviderReferenceNumber = HttpContext.GetRouteValueString(RouteValues.Ukprn);
        }
    }
}