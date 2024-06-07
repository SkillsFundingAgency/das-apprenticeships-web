﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Domain.Interfaces;
using SFA.DAS.Apprenticeships.Web.Infrastructure;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfPaymentStatus;
using SFA.DAS.Apprenticeships.Web.Services;
using SFA.DAS.Apprenticeships.Web.Extensions;

namespace SFA.DAS.Apprenticeships.Web.Controllers.ChangeOfPaymentStatus;

[Authorize(Policy = nameof(PolicyNames.HasEmployerAccount))]
[Route("employer/{employerAccountId}/ChangeOfPaymentStatus/{apprenticeshipHashedId}")]
public class ChangeOfPaymentStatusEmployerController : Controller
{
    private readonly ILogger<ChangeOfPaymentStatusEmployerController> _logger;
    private readonly IApprenticeshipService _apprenticeshipService;
    private readonly ICacheService _cache;
    private readonly UrlBuilder _externalEmployerUrlHelper;

    public const string FreezeProviderPaymentsViewName = "~/Views/ChangeOfPaymentStatus/FreezeProviderPayments.cshtml";
    public const string UnfreezeProviderPaymentsViewName = "~/Views/ChangeOfPaymentStatus/UnfreezeProviderPayments.cshtml";

    public ChangeOfPaymentStatusEmployerController(ILogger<ChangeOfPaymentStatusEmployerController> logger, IApprenticeshipService apprenticeshipService, ICacheService cacheService, UrlBuilder externalEmployerUrlHelper)
    {
        _logger = logger;
        _apprenticeshipService = apprenticeshipService;
        _cache = cacheService;
        _externalEmployerUrlHelper = externalEmployerUrlHelper;
    }

    [HttpGet]
    [Route("freeze")]
    public async Task<IActionResult> FreezeProviderPaymentsPage(string employerAccountId, string apprenticeshipHashedId)
    {
        var response = await _apprenticeshipService.GetApprenticeshipKey(apprenticeshipHashedId);
        if (response == Guid.Empty)
        {
            return NotFound();
        }

        var backLink = _externalEmployerUrlHelper.CommitmentsV2Link("ApprenticeDetails", employerAccountId, apprenticeshipHashedId.ToUpper());

        var model = new FreezeProviderPaymentsModel{ ApprenticeshipKey = response, ApprenticeshipHashedId = apprenticeshipHashedId, BackLinkUrl = backLink, EmployerAccountId = employerAccountId };

        return View(FreezeProviderPaymentsViewName, model);
    }

    [HttpPost]
    [Route("freeze")]
    public async Task<IActionResult> FreezeProviderPaymentsPage(FreezeProviderPaymentsModel model)
    {
        if (model.FreezePayments == true)
        {
            await _apprenticeshipService.FreezePayments(model.ApprenticeshipKey, model.ReasonForFreeze);

            return Redirect(_externalEmployerUrlHelper.CommitmentsV2Link("ApprenticeDetails", model.EmployerAccountId, model.ApprenticeshipHashedId?.ToUpper()) + "?showProviderPaymentsInactive=true");
        }

        return Redirect(_externalEmployerUrlHelper.CommitmentsV2Link("ApprenticeDetails", model.EmployerAccountId, model.ApprenticeshipHashedId?.ToUpper()));
    }

    [HttpGet]
    [Route("unfreeze")]
    public async Task<IActionResult> UnfreezeProviderPaymentsPage(string employerAccountId, string apprenticeshipHashedId)
    {
        var response = await _apprenticeshipService.GetApprenticeshipKey(apprenticeshipHashedId);
        if (response == Guid.Empty)
        {
            _logger.LogWarning("ApprenticeshipKey not found for apprenticeshipHashId {apprenticeshipHashId}", apprenticeshipHashedId);
            return NotFound();
        }

        var backLink = _externalEmployerUrlHelper.CommitmentsV2Link("ApprenticeDetails", employerAccountId, apprenticeshipHashedId.ToUpper());

        var model = new UnfreezeProviderPaymentsModel { ApprenticeshipKey = response, ApprenticeshipHashedId = apprenticeshipHashedId, BackLinkUrl = backLink, EmployerAccountId = employerAccountId };
        await _cache.SetCacheModelAsync(model);

        return View(UnfreezeProviderPaymentsViewName, model);
    }

    [HttpPost]
    [Route("unfreeze")]
    public async Task<IActionResult> UnfreezeProviderPaymentsPage(UnfreezeProviderPaymentsModel model)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("ModelState is invalid for {method} {validationErrors}", nameof(UnfreezeProviderPaymentsPage), ModelState.GetErrorSummary());
            return View(UnfreezeProviderPaymentsViewName, model);
        }

        if (model.UnfreezePayments == true)
        {
            _logger.LogInformation("Unfreezing payments for apprenticeship {apprenticeshipKey}", model.ApprenticeshipKey);
            await _apprenticeshipService.UnfreezePayments(model.ApprenticeshipKey);
            return Redirect(_externalEmployerUrlHelper.CommitmentsV2Link("ApprenticeDetails", model.EmployerAccountId, model.ApprenticeshipHashedId?.ToUpper()) + "?showProviderPaymentsActive=true");
        }

        _logger.LogInformation("Unfreeze payments not selected for apprenticeship {apprenticeshipKey}", model.ApprenticeshipKey);
        return Redirect(_externalEmployerUrlHelper.CommitmentsV2Link("ApprenticeDetails", model.EmployerAccountId, model.ApprenticeshipHashedId?.ToUpper()));
    }
}