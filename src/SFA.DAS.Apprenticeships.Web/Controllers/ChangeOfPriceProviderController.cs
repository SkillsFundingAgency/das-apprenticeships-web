﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;
using SFA.DAS.Apprenticeships.Domain.Interfaces;
using SFA.DAS.Apprenticeships.Web.Extensions;
using SFA.DAS.Apprenticeships.Web.Infrastructure;
using SFA.DAS.Apprenticeships.Web.Models;
using SFA.DAS.Apprenticeships.Web.Services;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.Provider.Shared.UI.Attributes;
using SFA.DAS.Provider.Shared.UI.Extensions;
using SFA.DAS.Provider.Shared.UI.Models;
using System.Web;
using NavigationSection = SFA.DAS.Provider.Shared.UI.NavigationSection;
using PriceChangeInitiatedBy = SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.InitiatedBy;

namespace SFA.DAS.Apprenticeships.Web.Controllers
{
    [Authorize]
    public class ChangeOfPriceProviderController : Controller
    {
        private readonly ILogger<ChangeOfPriceProviderController> _logger;
        private readonly IApprenticeshipService _apprenticeshipService;
        private readonly IMapper _mapper;
        private readonly IExternalUrlHelper _externalProviderUrlHelper;
        private readonly UrlBuilder _externalEmployerUrlHelper;
        private readonly ICacheService _cache;
        public const string ProviderEnterChangeDetailsViewName = "Provider/EnterChangeDetails";
        public const string ProviderCheckDetailsViewName = "Provider/CheckDetails";
		public const string ProviderCancelPendingChangeViewName = "Provider/CancelPendingChange";


		public ChangeOfPriceProviderController(
            ILogger<ChangeOfPriceProviderController> logger, 
            IApprenticeshipService apprenticeshipService, 
            IMapper mapper,
			ICacheService cache,
            IExternalUrlHelper externalProviderUrlHelper, UrlBuilder externalEmployerUrlHelper)
        {
            _logger = logger;
            _apprenticeshipService = apprenticeshipService;
            _mapper = mapper;
			_cache = cache;
            _externalProviderUrlHelper = externalProviderUrlHelper;
            _externalEmployerUrlHelper = externalEmployerUrlHelper;
        }
        
        [HttpGet]
        [SetNavigationSection(NavigationSection.ManageApprentices)]
        [Route("provider/{ukprn}/ChangeOfPrice/{apprenticeshipHashedId}")]
        public async Task<IActionResult> GetProviderEnterChangeDetails(string apprenticeshipHashedId)
        {
            var apprenticeshipPrice = await GetApprenticeshipPrice(apprenticeshipHashedId);
            if (apprenticeshipPrice == null)
            {
                return NotFound();
            }

            var model = _mapper.Map<ProviderChangeOfPriceModel>(apprenticeshipPrice);
            PopulateProviderInitiatedRouteValues(model);
            await _cache.SetCacheModelAsync(model);
            return View(ProviderEnterChangeDetailsViewName, model);
        }

        [HttpGet]
        [Route("provider/{ukprn}/ChangeOfPrice/{apprenticeshipHashedId}/edit")]
        public IActionResult GetProviderEditChangeDetails(ProviderChangeOfPriceModel model)
        {
            return View(ProviderEnterChangeDetailsViewName, model);
        }

        [HttpPost]
        [SetNavigationSection(NavigationSection.ManageApprentices)]
        [Route("provider/{ukprn}/ChangeOfPrice/{apprenticeshipHashedId}")]
        public async Task<IActionResult> ProviderCheckDetails(ProviderChangeOfPriceModel model)
        {
			PopulateProviderInitiatedRouteValues(model);
			if (!ModelState.IsValid)
            {
                return View(ProviderEnterChangeDetailsViewName, model);
            }

            await _cache.SetCacheModelAsync(model);
			return View(ProviderCheckDetailsViewName, model);
        }

        [HttpPost]
		[Route("provider/{ukprn}/ChangeOfPrice/{apprenticeshipHashedId}/submit")]
		public async Task<IActionResult> ProviderInitiatedSubmitChange(ProviderChangeOfPriceModel model)
		{
            await _apprenticeshipService.CreatePriceHistory(model.ApprenticeshipKey, "Provider", HttpContext.User.Identity?.Name!, model.ApprenticeshipTrainingPrice, model.ApprenticeshipEndPointAssessmentPrice, model.ApprenticeshipTotalPrice, HttpUtility.HtmlEncode(model.ReasonForChangeOfPrice), model.EffectiveFromDate.Date.GetValueOrDefault());

            var providerCommitmentsReturnUrl = _externalProviderUrlHelper.GenerateUrl(new UrlParameters
                { Controller = "", SubDomain = Subdomains.Approvals, RelativeRoute = $"{model.ProviderReferenceNumber}/apprentices/{model.ApprenticeshipHashedId}?showChangeOfPriceRequestSent=true" });
            return Redirect(providerCommitmentsReturnUrl);
		}

        [HttpGet]
		[SetNavigationSection(NavigationSection.ManageApprentices)]
		[Route("provider/{ukprn}/ChangeOfPrice/{apprenticeshipHashedId}/pending")]
		public async Task<IActionResult> GetViewPendingPriceChangePageProvider(long ukprn, string apprenticeshipHashedId)
		{
			var pendingPriceChange = await GetPendingPriceChange(apprenticeshipHashedId);
			if (pendingPriceChange == null || !pendingPriceChange.HasPendingPriceChange)
			{
				return NotFound();
			}

			return View(ProviderCancelPendingChangeViewName, new ProviderViewPendingPriceChangeModel(apprenticeshipHashedId, pendingPriceChange.PendingPriceChange, ukprn));
		}

        [HttpPost]
        [SetNavigationSection(NavigationSection.ManageApprentices)]
        [Route("provider/{ukprn}/ChangeOfPrice/{apprenticeshipHashedId}/pending")]
        public async Task<IActionResult> PostViewPendingPriceChangePage(long ukprn, string apprenticeshipHashedId, string CancelRequest)
        {
            if (CancelRequest != "1")
            {
                await _apprenticeshipService.ApprovePendingPriceChange(await _apprenticeshipService.GetApprenticeshipKey(apprenticeshipHashedId), HttpContext.User.Identity?.Name!);
                return Redirect(_externalProviderUrlHelper.GenerateUrl(new UrlParameters { Controller = "", SubDomain = Subdomains.Approvals, RelativeRoute = $"{ukprn}/apprentices/{apprenticeshipHashedId}" }));
            }

            var apprenticeshipKey = await _apprenticeshipService.GetApprenticeshipKey(apprenticeshipHashedId);
            if (apprenticeshipKey == default(Guid))
            {
                _logger.LogWarning($"Apprenticeship key not found for apprenticeship with hashed id {apprenticeshipHashedId}");
                return NotFound();
            }

            await _apprenticeshipService.CancelPendingPriceChange(apprenticeshipKey);
            return Redirect(_externalProviderUrlHelper.GenerateUrl(new UrlParameters { Controller = "", SubDomain = Subdomains.Approvals, RelativeRoute = $"{ukprn}/apprentices/{apprenticeshipHashedId}?showPriceChangeCancelled=true" }));
        }

        private async Task<ApprenticeshipPrice?> GetApprenticeshipPrice(string apprenticeshipHashedId)
        {
            var apprenticeshipKey = await _apprenticeshipService.GetApprenticeshipKey(apprenticeshipHashedId);
            if (apprenticeshipKey == default(Guid))
            {
                _logger.LogWarning($"Apprenticeship key not found for apprenticeship with hashed id {apprenticeshipHashedId}");
                return null;
            }

            var apprenticeshipPrice = await _apprenticeshipService.GetApprenticeshipPrice(apprenticeshipKey);
            if (apprenticeshipPrice == null || apprenticeshipPrice.ApprenticeshipKey != apprenticeshipKey)
            {
                _logger.LogWarning($"ApprenticeshipPrice not found for apprenticeshipKey {apprenticeshipKey}");
                return null;
            }

            return apprenticeshipPrice;
        }

		private async Task<GetPendingPriceChangeResponse?> GetPendingPriceChange(string apprenticeshipHashedId)
		{
			var apprenticeshipKey = await _apprenticeshipService.GetApprenticeshipKey(apprenticeshipHashedId);
			if (apprenticeshipKey == default(Guid))
			{
				_logger.LogWarning($"Apprenticeship key not found for apprenticeship with hashed id {apprenticeshipHashedId}");
				return null;
			}

			var pendingPriceChange = await _apprenticeshipService.GetPendingPriceChange(apprenticeshipKey);
			if (pendingPriceChange == null || !pendingPriceChange.HasPendingPriceChange)
			{
				_logger.LogWarning($"Pending Apprenticeship Price not found for apprenticeshipKey {apprenticeshipKey}");
				return null;
			}

			if (pendingPriceChange == null || !pendingPriceChange.HasPendingPriceChange)
			{
				_logger.LogWarning($"GetPendingPriceChange Response returned from API is null");
				return null;
			}

			return pendingPriceChange;
		}

		//  If other provider endpoints use the same route values, this could be refactored to take an interface/abstract class instead of CreateChangeOfPriceModel
		private void PopulateProviderInitiatedRouteValues(ProviderChangeOfPriceModel model)
        {
            model.ApprenticeshipHashedId = HttpContext.GetRouteValueString(RouteValues.ApprenticeshipHashedId);
            model.ProviderReferenceNumber =  long.Parse(HttpContext.GetRouteValueString(RouteValues.Ukprn));
        }
    }
}