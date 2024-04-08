using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;
using SFA.DAS.Apprenticeships.Domain.Interfaces;
using SFA.DAS.Apprenticeships.Web.Helpers;
using SFA.DAS.Apprenticeships.Web.Infrastructure;
using SFA.DAS.Apprenticeships.Web.Models;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfStartDate;
using SFA.DAS.Apprenticeships.Web.Services;
using SFA.DAS.Provider.Shared.UI;
using SFA.DAS.Provider.Shared.UI.Attributes;
using SFA.DAS.Provider.Shared.UI.Extensions;
using SFA.DAS.Provider.Shared.UI.Models;
using System.Web;

namespace SFA.DAS.Apprenticeships.Web.Controllers.ChangeOfStartDate;

[Authorize]
public class ChangeOfStartDateProviderController : Controller
{
    private readonly ILogger<ChangeOfStartDateProviderController> _logger;
    private readonly IApprenticeshipService _apprenticeshipService;
    private readonly IMapper _mapper;
    private readonly ICacheService _cache;
    private readonly IExternalUrlHelper _externalProviderUrlHelper;

    public const string EnterChangeDetailsViewName = "~/Views/ChangeOfStartDate/Provider/EnterChangeDetails.cshtml";
    public const string CheckDetailsViewName = "~/Views/ChangeOfStartDate/Provider/CheckDetails.cshtml";


    public ChangeOfStartDateProviderController(
        ILogger<ChangeOfStartDateProviderController> logger,
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
    [SetNavigationSection(NavigationSection.ManageApprentices)]
    [Route("provider/{ukprn}/ChangeOfStartDate/{apprenticeshipHashedId}")]
    public async Task<IActionResult> GetProviderEnterChangeDetails(string apprenticeshipHashedId)
    {
        var apprenticeshipStartDate = await GetApprenticeshipStartDate(apprenticeshipHashedId);
        if (apprenticeshipStartDate == null)
        {
            return NotFound();
        }

        var model = _mapper.Map<ProviderChangeOfStartDateModel>(apprenticeshipStartDate);
        RouteValuesHelper.PopulateProviderRouteValues(model, HttpContext);
        await _cache.SetCacheModelAsync(model);
        return View(EnterChangeDetailsViewName, model);
    }

    [HttpPost]
    [SetNavigationSection(NavigationSection.ManageApprentices)]
    [Route("provider/{ukprn}/ChangeOfStartDate/{apprenticeshipHashedId}")]
    public async Task<IActionResult> ProviderCheckDetails(ProviderChangeOfStartDateModel model)
    {
        RouteValuesHelper.PopulateProviderRouteValues(model, HttpContext);

        if (!ModelState.IsValid)
        {
            return View(EnterChangeDetailsViewName, model);
        }

        await _cache.SetCacheModelAsync(model);
        return View(CheckDetailsViewName, model);
    }

    [HttpPost]
    [SetNavigationSection(NavigationSection.ManageApprentices)]
    [Route("provider/{ukprn}/ChangeOfStartDate/{apprenticeshipHashedId}/submit")]
    public async Task<IActionResult> ProviderSubmitChangeDetails(ProviderChangeOfStartDateModel model)
    {
        await _apprenticeshipService.CreateStartDateChange(model.ApprenticeshipKey, "Provider", HttpContext.User.Identity?.Name!, HttpUtility.HtmlEncode(model.ReasonForChangeOfStartDate), model.ApprenticeshipActualStartDate!.Date.GetValueOrDefault());
        var providerCommitmentsReturnUrl = _externalProviderUrlHelper.GenerateUrl(new UrlParameters
        { 
            Controller = "", 
            SubDomain = Subdomains.Approvals, 
            RelativeRoute = $"{model.ProviderReferenceNumber}/apprentices/{model.ApprenticeshipHashedId?.ToUpper()}?banners=[changeOfStartDateSent]" 
        });

        return Redirect(providerCommitmentsReturnUrl);
    }

    private async Task<ApprenticeshipStartDate?> GetApprenticeshipStartDate(string apprenticeshipHashedId)
    {
        var apprenticeshipKey = await _apprenticeshipService.GetApprenticeshipKey(apprenticeshipHashedId);
        if (apprenticeshipKey == default)
        {
            _logger.LogWarning($"Apprenticeship key not found for apprenticeship with hashed id {apprenticeshipHashedId}");
            return null;
        }

        var apprenticeshipStartDate = await _apprenticeshipService.GetApprenticeshipStartDate(apprenticeshipKey);
        if (apprenticeshipStartDate == null || apprenticeshipStartDate.ApprenticeshipKey != apprenticeshipKey)
        {
            _logger.LogWarning($"ApprenticeshipStartDate not found for apprenticeshipKey {apprenticeshipKey}");
            return null;
        }

        return apprenticeshipStartDate;
    }
}