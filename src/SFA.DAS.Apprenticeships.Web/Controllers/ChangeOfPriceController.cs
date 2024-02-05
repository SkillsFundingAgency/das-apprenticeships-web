using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Domain.Interfaces;
using SFA.DAS.Apprenticeships.Web.Extensions;
using SFA.DAS.Apprenticeships.Web.Infrastructure;
using SFA.DAS.Apprenticeships.Web.Models;
using SFA.DAS.Apprenticeships.Web.Services;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.Provider.Shared.UI.Attributes;
using SFA.DAS.Provider.Shared.UI.Extensions;
using SFA.DAS.Provider.Shared.UI.Models;
using NavigationSection = SFA.DAS.Provider.Shared.UI.NavigationSection;

namespace SFA.DAS.Apprenticeships.Web.Controllers
{
    [Authorize]
    public class ChangeOfPriceController : Controller
    {
        private readonly ILogger<ChangeOfPriceController> _logger;
        private readonly IApprenticeshipService _apprenticeshipService;
        private readonly IMapper<CreateChangeOfPriceModel> _mapper;
        private readonly IExternalUrlHelper _externalProviderUrlHelper;
        private readonly UrlBuilder _externalEmployerUrlHelper;
        private readonly ICacheService _cache;
        public const string ProviderInitiatedViewName = "ProviderInitiated";
        public const string ProviderInitiatedCheckDetailsViewName = "ProviderInitiatedCheckDetails";
        public const string ProviderViewPendingViewName = "ProviderViewPending";
        public const string EmployerViewPendingViewName = "EmployerViewPending";

        public ChangeOfPriceController(
            ILogger<ChangeOfPriceController> logger, 
            IApprenticeshipService apprenticeshipService, 
            IMapper<CreateChangeOfPriceModel> mapper,
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
            await _apprenticeshipService.CreatePriceHistory(model.ApprenticeshipKey, model.ProviderReferenceNumber, null, HttpContext.User.Identity?.Name!, model.ApprenticeshipTrainingPrice, model.ApprenticeshipEndPointAssessmentPrice, model.ApprenticeshipTotalPrice, HttpUtility.HtmlEncode(model.ReasonForChangeOfPrice), model.EffectiveFromDate.Date.GetValueOrDefault());

            var providerCommitmentsReturnUrl = _externalProviderUrlHelper.GenerateUrl(new UrlParameters
                { Controller = "", SubDomain = Subdomains.Approvals, RelativeRoute = $"{model.ProviderReferenceNumber}/apprentices/{model.ApprenticeshipHashedId}?showChangeOfPriceRequestSent=true" });
            return Redirect(providerCommitmentsReturnUrl);
		}

		[HttpGet]
		[SetNavigationSection(NavigationSection.ManageApprentices)]
		[Route("provider/{ukprn}/ChangeOfPrice/{apprenticeshipHashedId}/pending")]
		public async Task<IActionResult> GetViewPendingPriceChangePageProvider(long ukprn, string apprenticeshipHashedId)
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

			return View(ProviderViewPendingViewName, new ProviderViewPendingPriceChangeModel(apprenticeshipKey, apprenticeshipHashedId, pendingPriceChange.PendingPriceChange, ukprn));
		}

        [HttpGet]
        [Authorize(Policy = nameof(PolicyNames.HasEmployerAccount))]
        [Route("employer/{employerAccountId}/ChangeOfPrice/{apprenticeshipHashedId}/pending")]
        public async Task<IActionResult> GetViewPendingPriceChangePageEmployer(string employerAccountId, string apprenticeshipHashedId)
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

            var backUrl = _externalEmployerUrlHelper.CommitmentsV2Link("ApprenticeDetails", employerAccountId, apprenticeshipHashedId);
            return View(EmployerViewPendingViewName, new EmployerViewPendingPriceChangeModel(apprenticeshipKey, apprenticeshipHashedId, pendingPriceChange.PendingPriceChange, employerAccountId, pendingPriceChange.ProviderName, backUrl));
        }

        [HttpPost]
        [SetNavigationSection(NavigationSection.ManageApprentices)]
        [Route("provider/{ukprn}/ChangeOfPrice/{apprenticeshipHashedId}/pending")]
        public async Task<IActionResult> PostViewPendingPriceChangePage(long ukprn, string apprenticeshipHashedId, string CancelRequest)
        {
            if (CancelRequest != "1")
            {
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

        [HttpPost]
        [Route("employer/{employerAccountId}/ChangeOfPrice/{apprenticeshipHashedId}/pending")]
        public async Task<IActionResult> PostViewPendingPriceChangePageEmployer(string employerAccountId, string apprenticeshipHashedId, string ApproveChanges, string rejectReason)
        {
            if (ApproveChanges != "0")
            {
                return Redirect(_externalEmployerUrlHelper.CommitmentsV2Link("ApprenticeDetails", employerAccountId, apprenticeshipHashedId));
            }

            var apprenticeshipKey = await _apprenticeshipService.GetApprenticeshipKey(apprenticeshipHashedId);
            if (apprenticeshipKey == default(Guid))
            {
                _logger.LogWarning($"Apprenticeship key not found for apprenticeship with hashed id {apprenticeshipHashedId}");
                return NotFound();
            }

            await _apprenticeshipService.RejectPendingPriceChange(apprenticeshipKey, rejectReason);
            return Redirect(_externalEmployerUrlHelper.CommitmentsV2Link("ApprenticeDetails", employerAccountId, apprenticeshipHashedId) + "?showPriceChangeRejected=true");
        }

        //  If other endpoints use the same route values, this could be refactored to take an interface/abstract class instead of CreateChangeOfPriceModel
        private void PopulateProviderInitiatedRouteValues(CreateChangeOfPriceModel model)
        {
            model.ApprenticeshipHashedId = HttpContext.GetRouteValueString(RouteValues.ApprenticeshipHashedId);
            model.ProviderReferenceNumber =  long.Parse(HttpContext.GetRouteValueString(RouteValues.Ukprn));
        }
    }
}