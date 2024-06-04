using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Domain;
using SFA.DAS.Apprenticeships.Domain.Interfaces;
using SFA.DAS.Apprenticeships.Web.Extensions;
using SFA.DAS.Apprenticeships.Web.Infrastructure;
using SFA.DAS.Apprenticeships.Web.Models;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfPrice;
using SFA.DAS.Apprenticeships.Web.Services;
using SFA.DAS.Employer.Shared.UI;
using System.Web;

namespace SFA.DAS.Apprenticeships.Web.Controllers.ChangeOfPrice;

[Authorize]
[Route("employer/{employerAccountId}/ChangeOfPrice/{apprenticeshipHashedId}")]
public class ChangeOfPriceEmployerController : Controller
{
    private readonly ILogger<ChangeOfPriceEmployerController> _logger;
    private readonly IApprenticeshipService _apprenticeshipService;
    private readonly IMapper _mapper;
    private readonly UrlBuilder _externalEmployerUrlHelper;
    private readonly ICacheService _cache;

    public const string EnterChangeDetailsViewName = "~/Views/ChangeOfPrice/Employer/EnterChangeDetails.cshtml";
    public const string CheckDetailsViewName = "~/Views/ChangeOfPrice/Employer/CheckDetails.cshtml";
    public const string CancelPendingChangeViewName = "~/Views/ChangeOfPrice/Employer/CancelPendingChange.cshtml";
    public const string ApproveProviderChangeOfPriceViewName = "~/Views/ChangeOfPrice/Employer/ApproveProviderChangeOfPrice.cshtml";


    public ChangeOfPriceEmployerController(
        ILogger<ChangeOfPriceEmployerController> logger,
        IApprenticeshipService apprenticeshipService,
        IMapper mapper,
        ICacheService cache,
        UrlBuilder externalEmployerUrlHelper)
    {
        _logger = logger;
        _apprenticeshipService = apprenticeshipService;
        _mapper = mapper;
        _cache = cache;
        _externalEmployerUrlHelper = externalEmployerUrlHelper;
    }

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetEmployerEnterChangeDetails(string apprenticeshipHashedId)
    {
        var apprenticeshipPrice = await _apprenticeshipService.GetApprenticeshipPrice(apprenticeshipHashedId);
        if (apprenticeshipPrice == null)
        {
            return NotFound();
        }

        var model = _mapper.Map<EmployerChangeOfPriceModel>(apprenticeshipPrice);
        HttpContext.PopulateEmployerInitiatedRouteValues(model);
        await _cache.SetCacheModelAsync(model);
        return View(EnterChangeDetailsViewName, model);
    }

    [HttpGet]
    [Route("edit")]
    public IActionResult GetEmployerEditChangeDetails(EmployerChangeOfPriceModel model)
    {
        return View(EnterChangeDetailsViewName, model);
    }

    [HttpPost]
    [Route("")]
    public async Task<IActionResult> EmployerCheckDetails(EmployerChangeOfPriceModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(EnterChangeDetailsViewName, model);
        }

        await _cache.SetCacheModelAsync(model);
        return View(CheckDetailsViewName, model);
    }

    [HttpPost]
    [Route("submit")]
    public async Task<IActionResult> EmployerInitiatedSubmitChange(EmployerChangeOfPriceModel model)
    {
        await _apprenticeshipService.CreatePriceHistory(model.ApprenticeshipKey, "Employer", HttpContext.User.GetUserId(), null, null, model.ApprenticeshipTotalPrice, HttpUtility.HtmlEncode(model.ReasonForChangeOfPrice), model.EffectiveFromDate.Date.GetValueOrDefault());

        var employerCommitmentsReturnUrl = $"{_externalEmployerUrlHelper.CommitmentsV2Link("ApprenticeDetails", model.EmployerAccountId, model.ApprenticeshipHashedId?.ToUpper())}?showChangeOfPriceRequestSent=true";
        return Redirect(employerCommitmentsReturnUrl);
    }

    [HttpGet]
    [Authorize(Policy = nameof(PolicyNames.HasEmployerAccount))]
    [Route("pending")]
    public async Task<IActionResult> ViewPendingPriceChangePage(string employerAccountId, string apprenticeshipHashedId)
    {
        var response = await _apprenticeshipService.GetPendingPriceChange(apprenticeshipHashedId);
        if (response == null || !response.HasPendingPriceChange)
        {
            return NotFound();
        }

        var backLink = _externalEmployerUrlHelper.CommitmentsV2Link("ApprenticeDetails", employerAccountId, apprenticeshipHashedId.ToUpper());

        switch (response.PendingPriceChange.Initiator.GetChangeInitiator())
        {
            case ChangeInitiator.Employer:
                var employerInitiateViewModel = _mapper.Map<EmployerCancelPriceChangeModel>(response);
                HttpContext.PopulateEmployerInitiatedRouteValues(employerInitiateViewModel);
                employerInitiateViewModel.BackLinkUrl = backLink;
                return View(CancelPendingChangeViewName, employerInitiateViewModel);

            case ChangeInitiator.Provider:
                var providerInitiateViewModel = _mapper.Map<EmployerViewPendingPriceChangeModel>(response);
                HttpContext.PopulateEmployerInitiatedRouteValues(providerInitiateViewModel);
                providerInitiateViewModel.BackLinkUrl = backLink;
                return View(ApproveProviderChangeOfPriceViewName, providerInitiateViewModel);

        }

        throw new ArgumentOutOfRangeException("Unrecognised PriceChangeInitiator");
    }

    [HttpPost]
    [Authorize(Policy = nameof(PolicyNames.HasEmployerAccount))]
    [Route("cancel")]
    public async Task<IActionResult> CancelPriceChange(string employerAccountId, string apprenticeshipHashedId, string cancelRequest)
    {
        var redirectUrl = _externalEmployerUrlHelper.CommitmentsV2Link("ApprenticeDetails", employerAccountId, apprenticeshipHashedId.ToUpper());
        if (cancelRequest != "1")
        {
            return Redirect(redirectUrl);
        }

        var apprenticeshipKey = await _apprenticeshipService.GetApprenticeshipKey(apprenticeshipHashedId);
        if (apprenticeshipKey == default)
        {
            _logger.LogWarning($"Apprenticeship key not found for apprenticeship with hashed id {apprenticeshipHashedId}");
            return NotFound();
        }

        await _apprenticeshipService.CancelPendingPriceChange(apprenticeshipKey);
        redirectUrl += "?showPriceChangeCancelled=true";
        return Redirect(redirectUrl);
    }

    [HttpPost]
    [Authorize(Policy = nameof(PolicyNames.HasEmployerAccount))]
    [Route("pending")]
    public async Task<IActionResult> ApproveOrRejectPriceChangePage(string employerAccountId, string apprenticeshipHashedId, string ApproveChanges, string rejectReason)
    {
        var apprenticeshipKey = await _apprenticeshipService.GetApprenticeshipKey(apprenticeshipHashedId);
        if (apprenticeshipKey == default)
        {
            _logger.LogWarning($"Apprenticeship key not found for apprenticeship with hashed id {apprenticeshipHashedId}");
            return NotFound();
        }

        if (ApproveChanges != "0")
        {
            var userId = HttpContext.User.GetUserId();
            await _apprenticeshipService.ApprovePendingPriceChange(apprenticeshipKey, userId);
            return Redirect(_externalEmployerUrlHelper.CommitmentsV2Link("ApprenticeDetails", employerAccountId, apprenticeshipHashedId.ToUpper()) + "?showPriceChangeApproved=true");
        }

        await _apprenticeshipService.RejectPendingPriceChange(apprenticeshipKey, rejectReason.HtmlEncode());
        return Redirect(_externalEmployerUrlHelper.CommitmentsV2Link("ApprenticeDetails", employerAccountId, apprenticeshipHashedId.ToUpper()) + "?showPriceChangeRejected=true");
    }

}