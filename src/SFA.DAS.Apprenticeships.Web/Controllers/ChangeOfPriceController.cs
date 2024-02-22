using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;
using SFA.DAS.Apprenticeships.Domain.Interfaces;
using SFA.DAS.Apprenticeships.Infrastructure;
using SFA.DAS.Apprenticeships.Web.Extensions;
using SFA.DAS.Apprenticeships.Web.Infrastructure;
using SFA.DAS.Apprenticeships.Web.Models;
using SFA.DAS.Apprenticeships.Web.Services;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.Provider.Shared.UI.Attributes;
using SFA.DAS.Provider.Shared.UI.Extensions;
using SFA.DAS.Provider.Shared.UI.Models;
using NavigationSection = SFA.DAS.Provider.Shared.UI.NavigationSection;
using PriceChangeInitiatedBy = SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.InitiatedBy;

namespace SFA.DAS.Apprenticeships.Web.Controllers
{
    [Authorize]
    public class ChangeOfPriceController : Controller
    {
        private readonly ILogger<ChangeOfPriceController> _logger;
        private readonly IApprenticeshipService _apprenticeshipService;
        private readonly IMapper _mapper;
        private readonly IExternalUrlHelper _externalProviderUrlHelper;
        private readonly UrlBuilder _externalEmployerUrlHelper;
        private readonly ICacheService _cache;
        public const string ProviderInitiatedViewName = "ProviderInitiated";
        public const string EmployerInitiatedViewName = "EmployerInitiated";
        public const string ProviderInitiatedCheckDetailsViewName = "ProviderInitiatedCheckDetails";
		public const string EmployerInitiatedCheckDetailsViewName = "EmployerInitiatedCheckDetails";
		public const string ProviderViewPendingViewName = "ProviderViewPending";
        public const string EmployerViewPendingViewName = "EmployerViewPending";
        public const string EmployerInitiatedEmployerViewPendingViewName = "EmployerInitiatedEmployerViewPending";


		public ChangeOfPriceController(
            ILogger<ChangeOfPriceController> logger, 
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
        public async Task<IActionResult> GetProviderInitiatedPage(string apprenticeshipHashedId)
        {
            var apprenticeshipPrice = await GetApprenticeshipPrice(apprenticeshipHashedId);
            if (apprenticeshipPrice == null)
            {
                return NotFound();
            }

            var model = _mapper.Map<ProviderChangeOfPriceModel>(apprenticeshipPrice);
            PopulateProviderInitiatedRouteValues(model);
            await _cache.SetCacheModelAsync(model);
            return View(ProviderInitiatedViewName, model);
        }

        [HttpGet]
        [SetNavigationSection(NavigationSection.ManageApprentices)]
        [Route("employer/{employerAccountId}/ChangeOfPrice/{apprenticeshipHashedId}")]
        public async Task<IActionResult> GetEmployerInitiatedPage(string apprenticeshipHashedId)
        {
            var apprenticeshipPrice = await GetApprenticeshipPrice(apprenticeshipHashedId);
            if (apprenticeshipPrice == null)
            {
                return NotFound();
            }

            var model = _mapper.Map<EmployerChangeOfPriceModel>(apprenticeshipPrice);
            PopulateEmployerInitiatedRouteValues(model);
            await _cache.SetCacheModelAsync(model);
            return View(EmployerInitiatedViewName, model);
        }

        [HttpGet]
        [Route("provider/{ukprn}/ChangeOfPrice/{apprenticeshipHashedId}/edit")]
        public IActionResult GetProviderInitiatedEditPage(ProviderChangeOfPriceModel model)
        {
            return View(ProviderInitiatedViewName, model);
        }

        [HttpPost]
        [SetNavigationSection(NavigationSection.ManageApprentices)]
        [Route("provider/{ukprn}/ChangeOfPrice/{apprenticeshipHashedId}")]
        public async Task<IActionResult> ProviderInitiatedCheckDetailsPage(ProviderChangeOfPriceModel model)
        {
			PopulateProviderInitiatedRouteValues(model);
			if (!ModelState.IsValid)
            {
                return View(ProviderInitiatedViewName, model);
            }

            await _cache.SetCacheModelAsync(model);
			return View(ProviderInitiatedCheckDetailsViewName, model);
        }

		[HttpGet]
		[Route("employer/{employerAccountId}/ChangeOfPrice/{apprenticeshipHashedId}/edit")]
		public IActionResult GetEmployerInitiatedEditPage(EmployerChangeOfPriceModel model)
		{
			return View(EmployerInitiatedViewName, model);
		}

		[HttpPost]
        [SetNavigationSection(NavigationSection.ManageApprentices)]
        [Route("employer/{employerAccountId}/ChangeOfPrice/{apprenticeshipHashedId}")]
        public async Task<IActionResult> EmployerInitiatedCheckDetailsPage(EmployerChangeOfPriceModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(EmployerInitiatedViewName, model);
            }

            await _cache.SetCacheModelAsync(model);
			return View(EmployerInitiatedCheckDetailsViewName, model);
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

        [HttpPost]
        [Route("employer/{employerAccountId}/ChangeOfPrice/{apprenticeshipHashedId}/submit")]
        public async Task<IActionResult> EmployerInitiatedSubmitChange(EmployerChangeOfPriceModel model)
        {
            await _apprenticeshipService.CreatePriceHistory(model.ApprenticeshipKey, "Employer", HttpContext.User.GetUserId(), null, null, model.ApprenticeshipTotalPrice, HttpUtility.HtmlEncode(model.ReasonForChangeOfPrice), model.EffectiveFromDate.Date.GetValueOrDefault());

            var employerCommitmentsReturnUrl = $"{_externalEmployerUrlHelper.CommitmentsV2Link("ApprenticeDetails", model.EmployerAccountId, model.ApprenticeshipHashedId)}?showChangeOfPriceRequestSent=true";
            return Redirect(employerCommitmentsReturnUrl);
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

			return View(ProviderViewPendingViewName, new ProviderViewPendingPriceChangeModel(apprenticeshipHashedId, pendingPriceChange.PendingPriceChange, ukprn));
		}

        [HttpGet]
        [Authorize(Policy = nameof(PolicyNames.HasEmployerAccount))]
        [Route("employer/{employerAccountId}/ChangeOfPrice/{apprenticeshipHashedId}/pending")]
        public async Task<IActionResult> GetViewPendingPriceChangePageEmployer(string employerAccountId, string apprenticeshipHashedId)
        {
            var response = await GetPendingPriceChange(apprenticeshipHashedId);
            if (response == null || !response.HasPendingPriceChange)
            {
                return NotFound();
            }

            var view = "";
            switch (response.PendingPriceChange.PriceChangeInitiatedBy())
            {
                case PriceChangeInitiatedBy.Employer:
                    view = EmployerInitiatedEmployerViewPendingViewName;
					break;

                case PriceChangeInitiatedBy.Provider:
                    view = EmployerViewPendingViewName;
					break;
            }

            var viewModel = _mapper.Map<EmployerViewPendingPriceChangeModel>(response);
            viewModel.BackLinkUrl = _externalEmployerUrlHelper.CommitmentsV2Link("ApprenticeDetails", employerAccountId, apprenticeshipHashedId);
            return View(view, viewModel);
        }

        [HttpPost]
        [Authorize(Policy = nameof(PolicyNames.HasEmployerAccount))]
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

        //  If other employer endpoints use the same route values, this could be refactored to take an interface/abstract class instead of EmployerChangeOfPriceModel
        private void PopulateEmployerInitiatedRouteValues(IEmployerRouteValues model)
        {
            model.ApprenticeshipHashedId = HttpContext.GetRouteValueString(RouteValues.ApprenticeshipHashedId);
            model.EmployerAccountId = HttpContext.GetRouteValueString(RouteValues.EmployerAccountId);
        }
    }
}