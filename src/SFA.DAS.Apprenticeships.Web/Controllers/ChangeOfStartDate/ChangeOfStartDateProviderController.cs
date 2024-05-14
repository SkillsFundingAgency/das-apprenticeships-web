using Microsoft.AspNetCore.Authorization;
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
    [SetNavigationSection(NavigationSection.ManageApprentices)]
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
	[SetNavigationSection(NavigationSection.ManageApprentices)]
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
    [SetNavigationSection(NavigationSection.ManageApprentices)]
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
    [SetNavigationSection(NavigationSection.ManageApprentices)]
    [Route("submit")]
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
                return View(ProviderCancelPendingChangeViewName, providerInitiateViewModel);

        }

        throw new ArgumentOutOfRangeException("ChangeInitiator");
    }

	[HttpPost]
	[SetNavigationSection(NavigationSection.ManageApprentices)]
	[Route("cancel")]
	public async Task<IActionResult> CancelStartDateChange(long ukprn, string apprenticeshipHashedId, string CancelRequest)
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