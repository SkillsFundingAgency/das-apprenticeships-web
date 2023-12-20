using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Domain.Interfaces;
using SFA.DAS.Apprenticeships.Web.Extensions;
using SFA.DAS.Apprenticeships.Web.Infrastructure;
using SFA.DAS.Apprenticeships.Web.Models;
using SFA.DAS.Apprenticeships.Web.Services;
using SFA.DAS.Provider.Shared.UI;
using SFA.DAS.Provider.Shared.UI.Attributes;
using SFA.DAS.Provider.Shared.UI.Extensions;
using SFA.DAS.Provider.Shared.UI.Models;

namespace SFA.DAS.Apprenticeships.Web.Controllers
{
	public class ChangeOfPriceController : Controller
    {
        private readonly ILogger<ChangeOfPriceController> _logger;
        private readonly IApprenticeshipService _apprenticeshipService;
        private readonly IMapper<CreateChangeOfPriceModel> _mapper;
        private readonly IExternalUrlHelper _externalUrlHelper;
        private readonly ICacheService _cache;
        public const string ProviderInitiatedViewName = "ProviderInitiated";
        public const string ProviderInitiatedCheckDetailsViewName = "ProviderInitiatedCheckDetails";
        public const string ViewPendingPriceViewName = "ViewPending";

        public ChangeOfPriceController(
            ILogger<ChangeOfPriceController> logger, 
            IApprenticeshipService apprenticeshipService, 
            IMapper<CreateChangeOfPriceModel> mapper,
			ICacheService cache,
            IExternalUrlHelper externalUrlHelper)
        {
            _logger = logger;
            _apprenticeshipService = apprenticeshipService;
            _mapper = mapper;
			_cache = cache;
            _externalUrlHelper = externalUrlHelper;
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
            model.ApprenticeshipKey = apprenticeshipKey;
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
		public async Task<IActionResult> ProviderInitiatedSubmitChange(CreateChangeOfPriceModel model)
		{
            await _apprenticeshipService.CreatePriceHistory(model.ApprenticeshipKey, model.ProviderReferenceNumber, null, "todo FLP-473", model.ApprenticeshipTrainingPrice, model.ApprenticeshipEndPointAssessmentPrice, model.ApprenticeshipTotalPrice, "todo FLP-354", model.EffectiveFromDate.Date.GetValueOrDefault());

            var providerCommitmentsReturnUrl = _externalUrlHelper.GenerateUrl(new UrlParameters
                { Controller = "", SubDomain = "pas", RelativeRoute = $"{model.ProviderReferenceNumber}/apprentices/{model.ApprenticeshipHashedId}?showChangeOfPriceRequestSent=true" });
            return Redirect(providerCommitmentsReturnUrl);
		}

		[HttpGet]
		[SetNavigationSection(NavigationSection.ManageApprentices)]
		[Route("provider/{ukprn}/ChangeOfPrice/{apprenticeshipHashedId}/pending")]
		public async Task<IActionResult> GetViewPendingPriceChangePage(string apprenticeshipHashedId)
		{
			var apprenticeshipKey = await _apprenticeshipService.GetApprenticeshipKey(apprenticeshipHashedId);
			if (apprenticeshipKey == default(Guid))
			{
				_logger.LogWarning($"Apprenticeship key not found for apprenticeship with hashed id {apprenticeshipHashedId}");
				return NotFound();
			}

			var pendingPriceChange = await _apprenticeshipService.GetPendingPriceChange(apprenticeshipKey);
			if (pendingPriceChange == null || !pendingPriceChange.HasPendingPriceChange)
			{
				_logger.LogWarning($"Pending Apprenticeship Price not found for apprenticeshipKey {apprenticeshipKey}");
				return NotFound();
			}

			return View(ViewPendingPriceViewName, new ViewPendingPriceChangeModel(pendingPriceChange.PendingPriceChange));
		}

		//  If other endpoints use the same route values, this could be refactored to take an interface/abstract class instead of CreateChangeOfPriceModel
		private void PopulateProviderInitiatedRouteValues(CreateChangeOfPriceModel model)
        {
            model.ApprenticeshipHashedId = HttpContext.GetRouteValueString(RouteValues.ApprenticeshipHashedId);
            model.ProviderReferenceNumber =  long.Parse(HttpContext.GetRouteValueString(RouteValues.Ukprn));
        }
    }
}