using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;
using SFA.DAS.Apprenticeships.Domain.Interfaces;
using SFA.DAS.Apprenticeships.Web.Helpers;
using SFA.DAS.Apprenticeships.Web.Models;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfStartDate;
using SFA.DAS.Apprenticeships.Web.Services;
using SFA.DAS.Provider.Shared.UI;
using SFA.DAS.Provider.Shared.UI.Attributes;
using SFA.DAS.Provider.Shared.UI.Extensions;

namespace SFA.DAS.Apprenticeships.Web.Controllers.ChangeOfStartDate;

[Authorize]
public class ChangeOfStartDateProviderController : Controller
{
    private readonly ILogger<ChangeOfStartDateProviderController> _logger;
    private readonly IApprenticeshipService _apprenticeshipService;
    private readonly IMapper _mapper;
    private readonly IExternalUrlHelper _externalProviderUrlHelper;
    private readonly ICacheService _cache;

    public const string ProviderEnterChangeDetailsViewName =
        "~/Views/ChangeOfStartDate/Provider/EnterChangeDetails.cshtml";


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
        RouteValuesHelper.PopulateRouteValues(model, HttpContext);
        await _cache.SetCacheModelAsync(model);
        return View(ProviderEnterChangeDetailsViewName, model);
    }

    [HttpPost]
    [SetNavigationSection(NavigationSection.ManageApprentices)]
    [Route("provider/{ukprn}/ChangeOfStartDate/{apprenticeshipHashedId}")]
    public async Task<IActionResult> ProviderSubmitChangeDetails(ProviderChangeOfStartDateModel model)
    {
        RouteValuesHelper.PopulateRouteValues(model, HttpContext);

        if (!ModelState.IsValid)
        {
            return View(ProviderEnterChangeDetailsViewName, model);
        }

        await _cache.SetCacheModelAsync(model);
        throw new NotImplementedException("TODO later story continue journey");
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