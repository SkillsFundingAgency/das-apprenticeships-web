using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Application.Exceptions;
using SFA.DAS.Apprenticeships.Domain;
using SFA.DAS.Apprenticeships.Domain.Interfaces;
using SFA.DAS.Apprenticeships.Web.Extensions;
using SFA.DAS.Apprenticeships.Web.Infrastructure;
using SFA.DAS.Apprenticeships.Web.Models;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfStartDate;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.Apprenticeships.Web.Models.Enums;
using SFA.DAS.Apprenticeships.Web.Constants.Employer;
using SFA.DAS.Apprenticeships.Web.Services;

namespace SFA.DAS.Apprenticeships.Web.Controllers.ChangeOfStartDate;

[Authorize]
[Route("employer/{employerAccountId}/ChangeOfStartDate/{apprenticeshipHashedId}")]
public class ChangeOfStartDateEmployerController : Controller
{
    private readonly ILogger<ChangeOfStartDateEmployerController> _logger;
    private readonly IApprenticeshipService _apprenticeshipService;
    private readonly IMapper _mapper;
    private readonly UrlBuilder _externalEmployerUrlHelper;
    private readonly ICacheService _cache;
    public const string ApproveProviderChangeOfStartDateViewName = "~/Views/ChangeOfStartDate/Employer/ApproveProviderChangeOfStartDate.cshtml";

    public ChangeOfStartDateEmployerController(
        ILogger<ChangeOfStartDateEmployerController> logger, 
        IApprenticeshipService apprenticeshipService,
        IMapper mapper,
        UrlBuilder externalEmployerUrlHelper,
        ICacheService cache)
	{
        _logger = logger;
        _apprenticeshipService = apprenticeshipService;
        _mapper = mapper;
        _externalEmployerUrlHelper = externalEmployerUrlHelper;
        _cache = cache;
    }

    [HttpGet]
    [Authorize(Policy = nameof(PolicyNames.HasEmployerAccount))]
    [Route("pending")]
    public async Task<IActionResult> ViewPendingChangePage(string employerAccountId, string apprenticeshipHashedId)
    {
        var response = await _apprenticeshipService.GetPendingStartDateChange(apprenticeshipHashedId);
        if (response == null)
        {
            return NotFound();
        }

        var backLink = _externalEmployerUrlHelper.CommitmentsV2Link(EmployerRoutes.ApprenticeDetails, employerAccountId, apprenticeshipHashedId.ToUpper());

        switch (response.PendingStartDateChange!.Initiator.GetChangeInitiator())
        {
            case ChangeInitiator.Employer:
                throw new NotImplementedException("Employer initiated change of start date is not yet implemented");

            case ChangeInitiator.Provider:
                var providerInitiateViewModel = _mapper.Map<EmployerViewPendingStartDateChangeModel>(response);
                HttpContext.PopulateEmployerInitiatedRouteValues(providerInitiateViewModel);
                providerInitiateViewModel.BackLinkUrl = backLink;
                await _cache.SetCacheModelAsync(providerInitiateViewModel);
                return View(ApproveProviderChangeOfStartDateViewName, providerInitiateViewModel);

        }

        throw new ServiceException("Unrecognised ChangeInitiator");
    }

    [HttpPost]
    [Authorize(Policy = nameof(PolicyNames.HasEmployerAccount))]
    [Route("pending")]
    public async Task<IActionResult> ApproveOrRejectStartDateChange(EmployerViewPendingStartDateChangeModel model)
    {
        if (!ModelState.IsValid)
            return View(ApproveProviderChangeOfStartDateViewName, model);

        var redirectUrl = _externalEmployerUrlHelper.CommitmentsV2Link(EmployerRoutes.ApprenticeDetails, model.EmployerAccountId, model.ApprenticeshipHashedId!.ToUpper());

        if (model.ApproveRequest != "0")
        {
            var userId = HttpContext.User.GetUserId();
            await _apprenticeshipService.ApprovePendingStartDateChange(model.ApprenticeshipKey, userId!);

            redirectUrl = redirectUrl.AppendEmployerBannersToUrl(EmployerApprenticeDetailsBanners.ChangeOfStartDateApproved);
            return Redirect(redirectUrl);
		}

		await _apprenticeshipService.RejectPendingStartDateChange(model.ApprenticeshipKey, model.RejectReason ?? string.Empty);

        redirectUrl = redirectUrl.AppendEmployerBannersToUrl(EmployerApprenticeDetailsBanners.ChangeOfStartDateRejected);
        return Redirect(redirectUrl);
	}

}
