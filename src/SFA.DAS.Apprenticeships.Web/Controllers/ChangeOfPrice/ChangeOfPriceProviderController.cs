using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Application.Exceptions;
using SFA.DAS.Apprenticeships.Domain;
using SFA.DAS.Apprenticeships.Domain.Interfaces;
using SFA.DAS.Apprenticeships.Web.Extensions;
using SFA.DAS.Apprenticeships.Web.Helpers;
using SFA.DAS.Apprenticeships.Web.Infrastructure;
using SFA.DAS.Apprenticeships.Web.Models;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfPrice;
using SFA.DAS.Apprenticeships.Web.Models.Enums;
using SFA.DAS.Apprenticeships.Web.Services;
using SFA.DAS.Provider.Shared.UI.Extensions;
using SFA.DAS.Provider.Shared.UI.Models;
using System.Web;

namespace SFA.DAS.Apprenticeships.Web.Controllers.ChangeOfPrice;

[Authorize]
[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
[Route("provider/{ukprn}/ChangeOfPrice/{apprenticeshipHashedId}")]
public class ChangeOfPriceProviderController : Controller
{
    private readonly ILogger<ChangeOfPriceProviderController> _logger;
    private readonly IApprenticeshipService _apprenticeshipService;
    private readonly IMapper _mapper;
    private readonly IExternalUrlHelper _externalProviderUrlHelper;
    private readonly ICacheService _cache;
    public const string ProviderEnterChangeDetailsViewName = "~/Views/ChangeOfPrice/Provider/EnterChangeDetails.cshtml";
    public const string ProviderCheckDetailsViewName = "~/Views/ChangeOfPrice/Provider/CheckDetails.cshtml";
    public const string ProviderCancelPendingChangeViewName = "~/Views/ChangeOfPrice/Provider/CancelPendingChange.cshtml";
    public const string ApproveEmployerChangeOfPriceViewName = "~/Views/ChangeOfPrice/Provider/ApproveEmployerChangeOfPrice.cshtml";
    public const string ProviderConfirmPriceBreakdownViewName = "~/Views/ChangeOfPrice/Provider/ConfirmPriceBreakdown.cshtml";


    public ChangeOfPriceProviderController(
        ILogger<ChangeOfPriceProviderController> logger,
        IApprenticeshipService apprenticeshipService,
        IMapper mapper,
        ICacheService cache,
        IExternalUrlHelper externalProviderUrlHelper)
    {
        _logger = logger;
        _apprenticeshipService = apprenticeshipService;
        _mapper = mapper;
        _cache = cache;
        _externalProviderUrlHelper = externalProviderUrlHelper;
    }

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetProviderEnterChangeDetails(string apprenticeshipHashedId)
    {
        var apprenticeshipPrice = await _apprenticeshipService.GetApprenticeshipPrice(apprenticeshipHashedId);
        if (apprenticeshipPrice == null)
        {
            return NotFound();
        }

        var model = _mapper.Map<ProviderChangeOfPriceModel>(apprenticeshipPrice);
        RouteValuesHelper.PopulateProviderRouteValues(model, HttpContext);
        await _cache.SetCacheModelAsync(model);
        return View(ProviderEnterChangeDetailsViewName, model);
    }

    [HttpGet]
    [Route("edit")]
    public IActionResult GetProviderEditChangeDetails(ProviderChangeOfPriceModel model)
    {
        return View(ProviderEnterChangeDetailsViewName, model);
    }

    [HttpPost]
    [Route("")]
    public async Task<IActionResult> ProviderCheckDetails(ProviderChangeOfPriceModel model)
    {
        RouteValuesHelper.PopulateProviderRouteValues(model, HttpContext);
        if (!ModelState.IsValid)
        {
            return View(ProviderEnterChangeDetailsViewName, model);
        }

        await _cache.SetCacheModelAsync(model);
        return View(ProviderCheckDetailsViewName, model);
    }

    [HttpPost]
    [Route("submit")]
    public async Task<IActionResult> ProviderInitiatedSubmitChange(ProviderChangeOfPriceModel model)
    {
        var priceChangeStatus = await _apprenticeshipService.CreatePriceHistory(model.ApprenticeshipKey, "Provider", HttpContext.User.Identity?.Name!, model.ApprenticeshipTrainingPrice, model.ApprenticeshipEndPointAssessmentPrice, model.ApprenticeshipTotalPrice, HttpUtility.HtmlEncode(model.ReasonForChangeOfPrice), model.EffectiveFromDate.Date.GetValueOrDefault());
        string providerCommitmentsReturnUrl;

        if (priceChangeStatus == "Approved")
        {
            providerCommitmentsReturnUrl = _externalProviderUrlHelper
                .GenerateUrl(new UrlParameters { Controller = "", SubDomain = Subdomains.Approvals, RelativeRoute = $"{model.ProviderReferenceNumber}/apprentices/{model.ApprenticeshipHashedId?.ToUpper()}"})
                .AppendProviderBannersToUrl(ProviderApprenticeDetailsBanners.ChangeOfPriceAutoApproved);
        }
        else
        {
            providerCommitmentsReturnUrl = _externalProviderUrlHelper
                .GenerateUrl(new UrlParameters { Controller = "", SubDomain = Subdomains.Approvals, RelativeRoute = $"{model.ProviderReferenceNumber}/apprentices/{model.ApprenticeshipHashedId?.ToUpper()}" })
                .AppendProviderBannersToUrl(ProviderApprenticeDetailsBanners.ChangeOfPriceRequestSent);
        }

        return Redirect(providerCommitmentsReturnUrl);
    }

    [HttpGet]
    [Route("pending")]
    public async Task<IActionResult> ViewPendingPriceChangePage(long ukprn, string apprenticeshipHashedId)
    {
        var response = await _apprenticeshipService.GetPendingPriceChange(apprenticeshipHashedId);
        if (response == null || !response.HasPendingPriceChange)
        {
            return NotFound();
        }

        switch (response.PendingPriceChange.Initiator.GetChangeInitiator())
        {
            case ChangeInitiator.Employer:
                var employerInitiateViewModel = _mapper.Map<ProviderViewPendingPriceChangeModel>(response);
                RouteValuesHelper.PopulateProviderRouteValues(employerInitiateViewModel, HttpContext);
                await _cache.SetCacheModelAsync(employerInitiateViewModel);
                return View(ApproveEmployerChangeOfPriceViewName, employerInitiateViewModel);

            case ChangeInitiator.Provider:
                var providerInitiateViewModel = _mapper.Map<ProviderCancelPriceChangeModel>(response);
                RouteValuesHelper.PopulateProviderRouteValues(providerInitiateViewModel, HttpContext);
                await _cache.SetCacheModelAsync(providerInitiateViewModel);
                return View(ProviderCancelPendingChangeViewName, providerInitiateViewModel);

        }

        throw new ServiceException("Unrecognised PriceChangeInitiator");
    }

    [HttpPost]
    [Route("pending")]
    public async Task<IActionResult> ApproveOrRejectPendingPriceChange(ProviderViewPendingPriceChangeModel model)
    {
        if (!ModelState.IsValid)
            return View(ApproveEmployerChangeOfPriceViewName, model);

        if (model.ApproveRequest == "1")
        {
            return RedirectToAction("ConfirmPriceBreakdown", new { ukprn = model.ProviderReferenceNumber, apprenticeshipHashedId = model.ApprenticeshipHashedId });
        }

        await _apprenticeshipService.RejectPendingPriceChange(model.ApprenticeshipKey, model.RejectReason!.HtmlEncode());

        var redirectUrl = _externalProviderUrlHelper
            .GenerateUrl(new UrlParameters { Controller = "", SubDomain = Subdomains.Approvals, RelativeRoute = $"{model.ProviderReferenceNumber}/apprentices/{model.ApprenticeshipHashedId!.ToUpper()}" })
            .AppendProviderBannersToUrl(ProviderApprenticeDetailsBanners.ChangeOfPriceRejected);
        return Redirect(redirectUrl);
    }

    [HttpGet]
    [Route("approve")]
    public async Task<IActionResult> ConfirmPriceBreakdown(long ukprn, string apprenticeshipHashedId)
    {
        var response = await _apprenticeshipService.GetPendingPriceChange(apprenticeshipHashedId);

        var confirmPriceBreakdownPriceChangeModel = _mapper.Map<ProviderConfirmPriceBreakdownPriceChangeModel>(response!);

        return View(ProviderConfirmPriceBreakdownViewName, confirmPriceBreakdownPriceChangeModel);
    }

    [HttpPost]
    [Route("approve")]
    public async Task<IActionResult> ConfirmApprovePendingPriceChange(ProviderConfirmPriceBreakdownPriceChangeModel model, long ukprn, string apprenticeshipHashedId)
    {
        if (!ModelState.IsValid)
        {
            return View(ProviderConfirmPriceBreakdownViewName, model);
        }

        var userId = HttpContext.User.Identity?.Name!;
        await _apprenticeshipService.ApprovePendingPriceChange(model.ApprenticeshipKey, userId, model.ApprenticeshipTrainingPrice.GetValueOrDefault(), model.ApprenticeshipEndPointAssessmentPrice.GetValueOrDefault());

        var redirectUrl = _externalProviderUrlHelper
            .GenerateUrl(new UrlParameters { Controller = "", SubDomain = Subdomains.Approvals, RelativeRoute = $"{ukprn}/apprentices/{apprenticeshipHashedId.ToUpper()}" })
            .AppendProviderBannersToUrl(ProviderApprenticeDetailsBanners.ChangeOfPriceApproved);
        return Redirect(redirectUrl);
    }


    [HttpPost]
    [Route("cancel")]
    public async Task<IActionResult> CancelPriceChange(ProviderCancelPriceChangeModel model)
    {
        if (!ModelState.IsValid)
            return View(ProviderCancelPendingChangeViewName, model);

        var redirectUrl = _externalProviderUrlHelper.GenerateUrl(new UrlParameters { Controller = "", SubDomain = Subdomains.Approvals, RelativeRoute = $"{model.ProviderReferenceNumber}/apprentices/{model.ApprenticeshipHashedId}" });

        if (model.CancelRequest != "1")
        {
            return Redirect(redirectUrl);
        }

        await _apprenticeshipService.CancelPendingPriceChange(model.ApprenticeshipKey);

        redirectUrl = redirectUrl.AppendProviderBannersToUrl(ProviderApprenticeDetailsBanners.ChangeOfPriceCancelled);
        return Redirect(redirectUrl);
    }
}