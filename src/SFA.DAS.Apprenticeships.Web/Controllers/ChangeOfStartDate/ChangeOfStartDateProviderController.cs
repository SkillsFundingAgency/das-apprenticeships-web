﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Domain;
using SFA.DAS.Apprenticeships.Domain.Interfaces;
using SFA.DAS.Apprenticeships.Web.Helpers;
using SFA.DAS.Apprenticeships.Web.Infrastructure;
using SFA.DAS.Apprenticeships.Web.Models;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfStartDate;
using SFA.DAS.Apprenticeships.Web.Models.Enums;
using SFA.DAS.Apprenticeships.Web.Services;
using SFA.DAS.Provider.Shared.UI.Extensions;
using SFA.DAS.Provider.Shared.UI.Models;
using System.Web;
using SFA.DAS.Apprenticeships.Application.Exceptions;
using SFA.DAS.Apprenticeships.Web.Extensions;
using System.Reflection;

namespace SFA.DAS.Apprenticeships.Web.Controllers.ChangeOfStartDate;

[Authorize]
[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
[Route("provider/{ukprn}/ChangeOfStartDate/{apprenticeshipHashedId}")]
public class ChangeOfStartDateProviderController : Controller
{
    private readonly ILogger<ChangeOfStartDateProviderController> _logger;
    private readonly IApprenticeshipService _apprenticeshipService;
    private readonly IMapper _mapper;
    private readonly ICacheService _cache;
    private readonly IExternalUrlHelper _externalProviderUrlHelper;

    public const string EnterNewStartDateViewName = "~/Views/ChangeOfStartDate/Provider/EnterNewStartDate.cshtml";
	public const string EnterNewEndDateViewName = "~/Views/ChangeOfStartDate/Provider/EnterNewEndDate.cshtml";
	public const string CheckDetailsViewName = "~/Views/ChangeOfStartDate/Provider/CheckDetails.cshtml";
    public const string ProviderCancelPendingChangeViewName = "~/Views/ChangeOfStartDate/Provider/CancelPendingChange.cshtml";

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
    [Route("")]
    public async Task<IActionResult> GetEnterStartDatePage(string apprenticeshipHashedId)
    {
        var apprenticeshipStartDate = await _apprenticeshipService.GetApprenticeshipStartDate(apprenticeshipHashedId);
        if (apprenticeshipStartDate == null)
        {
            return NotFound();
        }

        var model = _mapper.Map<ProviderChangeOfStartDateModel>(apprenticeshipStartDate);
        RouteValuesHelper.PopulateProviderRouteValues(model, HttpContext);
        await _cache.SetCacheModelAsync(model);
        return View(EnterNewStartDateViewName, model);
    }

	[HttpPost]
	[Route("")]
	public async Task<IActionResult> SubmitStartDate(ProviderChangeOfStartDateModel model)
	{
        RouteValuesHelper.PopulateProviderRouteValues(model, HttpContext);

        if (!ModelState.IsValid)
        {
            return View(EnterNewStartDateViewName, model);
        }

        await _cache.SetCacheModelAsync(model);
        var providerPlannedEndDateModel = _mapper.Map<ProviderPlannedEndDateModel>(model);
        return View(EnterNewEndDateViewName, providerPlannedEndDateModel);
    }

	[HttpGet]
    [Route("edit")]
    public IActionResult GetProviderEditChangeDetails(ProviderPlannedEndDateModel model)
    {
        var view = HttpContext.Request.Query["view"].ToString();

        switch(view)
        {
            case "startDate":
                return View(EnterNewStartDateViewName, model);
            case "endDate":
                return View(EnterNewEndDateViewName, model);

        }

        _logger.LogError("Invalid view {view} requested for edit", view);
        return NotFound();
    }

    [HttpPost]
    [Route("checkDetails")]
    public async Task<IActionResult> ProviderCheckDetails(ProviderPlannedEndDateModel model)
    {
        //  Handle endDate input
        RouteValuesHelper.PopulateProviderRouteValues(model, HttpContext);

        if (!ModelState.IsValid)
        {
            return View(EnterNewEndDateViewName, model);
        }

        //  Apply endDate to change model
        if(model.UseSuggestedDate == true)
        {
            model.PlannedEndDate = new DateField(model.SuggestedEndDate);
        }

        await _cache.SetCacheModelAsync(model);

        return View(CheckDetailsViewName, model);
    }

    [HttpPost]
    [Route("submit")]
    public async Task<IActionResult> ProviderSubmitChangeDetails(ProviderChangeOfStartDateModel model)
    {
        await _apprenticeshipService.CreateStartDateChange(
            model.ApprenticeshipKey, 
            "Provider", 
            HttpContext.User.Identity?.Name!, 
            HttpUtility.HtmlEncode(model.ReasonForChangeOfStartDate), 
            model.ApprenticeshipActualStartDate!.Date.GetValueOrDefault(),
            model.PlannedEndDate!.Date.GetValueOrDefault());

        var providerCommitmentsReturnUrl = _externalProviderUrlHelper.GenerateUrl(new UrlParameters
        { 
            Controller = "", 
            SubDomain = Subdomains.Approvals, 
            RelativeRoute = $"{model.ProviderReferenceNumber}/apprentices/{model.ApprenticeshipHashedId?.ToUpper()}"
        }).AppendProviderBannersToUrl(ProviderApprenticeDetailsBanners.ChangeOfStartDateSent);

        return Redirect(providerCommitmentsReturnUrl);
    }

    [HttpGet]
    [Route("pending")]
    public async Task<IActionResult> ViewPendingChangePage(long ukprn, string apprenticeshipHashedId)
    {
        var response = await _apprenticeshipService.GetPendingStartDateChange(apprenticeshipHashedId);
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
                await _cache.SetCacheModelAsync(providerInitiateViewModel);
                return View(ProviderCancelPendingChangeViewName, providerInitiateViewModel);

        }

        throw new ServiceException("Unrecognised ChangeInitiator");
    }

	[HttpPost]
	[Route("cancel")]
	public async Task<IActionResult> CancelStartDateChange(ProviderCancelStartDateModel model)
	{
        if (!ModelState.IsValid)
            return View(ProviderCancelPendingChangeViewName, model);

        var redirectUrl = _externalProviderUrlHelper.GenerateUrl(new UrlParameters { Controller = "", SubDomain = Subdomains.Approvals, RelativeRoute = $"{model.ProviderReferenceNumber}/apprentices/{model.ApprenticeshipHashedId}" });

        if (model.CancelRequest != "1")
		{
			return Redirect(redirectUrl);
		}

		await _apprenticeshipService.CancelPendingStartDateChange(model.ApprenticeshipKey);

        redirectUrl = redirectUrl.AppendProviderBannersToUrl(ProviderApprenticeDetailsBanners.ChangeOfStartDateCancelled);
        return Redirect(redirectUrl);
    }

}