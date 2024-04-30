﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Domain;
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
public class ChangeOfStartDateProviderController : BaseChangeOfStartDateController<ChangeOfStartDateProviderController>
{
    private readonly ILogger<ChangeOfStartDateProviderController> _logger;
    private readonly IApprenticeshipService _apprenticeshipService;
    private readonly IMapper _mapper;
    private readonly ICacheService _cache;
    private readonly IExternalUrlHelper _externalProviderUrlHelper;

    public const string EnterChangeDetailsViewName = "~/Views/ChangeOfStartDate/Provider/EnterChangeDetails.cshtml";
    public const string CheckDetailsViewName = "~/Views/ChangeOfStartDate/Provider/CheckDetails.cshtml";
    public const string ProviderCancelPendingChangeViewName = "~/Views/ChangeOfStartDate/Provider/CancelPendingChange.cshtml";

    public ChangeOfStartDateProviderController(
        ILogger<ChangeOfStartDateProviderController> logger,
        IApprenticeshipService apprenticeshipService,
        IMapper mapper,
        ICacheService cache,
        IExternalUrlHelper externalProviderUrlHelper) : base(logger, apprenticeshipService)
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
            RelativeRoute = $"{model.ProviderReferenceNumber}/apprentices/{model.ApprenticeshipHashedId?.ToUpper()}?banners=ChangeOfStartDateSent" 
        });

        return Redirect(providerCommitmentsReturnUrl);
    }

    [HttpGet]
    [SetNavigationSection(NavigationSection.ManageApprentices)]
    [Route("provider/{ukprn}/ChangeOfStartDate/{apprenticeshipHashedId}/pending")]
    public async Task<IActionResult> ViewPendingPriceChangePage(long ukprn, string apprenticeshipHashedId)
    {
        var response = await GetPendingStartDateChange(apprenticeshipHashedId);
        if (response == null || !response.HasPendingStartDateChange)
        {
            return NotFound();
        }

        switch (response.PendingStartDateChange!.Initiator.GetChangeInitiator())
        {
            case ChangeInitiator.Employer:
                throw new NotImplementedException("Employer initiated change of start date is not yet implemented");

            case ChangeInitiator.Provider:
                var providerInitiateViewModel = _mapper.Map<ProviderCancelStartDateModel>(response);
                RouteValuesHelper.PopulateProviderRouteValues(providerInitiateViewModel, HttpContext);
                return View(ProviderCancelPendingChangeViewName, providerInitiateViewModel);

        }

        throw new ArgumentOutOfRangeException("ChangeInitiator");
    }

	[HttpPost]
	[SetNavigationSection(NavigationSection.ManageApprentices)]
	[Route("provider/{ukprn}/ChangeOfStartDate/{apprenticeshipHashedId}/cancel")]
	public async Task<IActionResult> CancelPriceChange(long ukprn, string apprenticeshipHashedId, string CancelRequest)
	{
		if (CancelRequest != "1")
		{
			return Redirect(_externalProviderUrlHelper.GenerateUrl(new UrlParameters { Controller = "", SubDomain = Subdomains.Approvals, RelativeRoute = $"{ukprn}/apprentices/{apprenticeshipHashedId}" }));
		}

		var apprenticeshipKey = await _apprenticeshipService.GetApprenticeshipKey(apprenticeshipHashedId);
		if (apprenticeshipKey == default)
		{
			_logger.LogWarning("Apprenticeship key not found for apprenticeship with hashed id {apprenticeshipHashedId}", apprenticeshipHashedId);
			return NotFound();
		}

		throw new NotImplementedException("To be completed in FLP-486");
	}
}