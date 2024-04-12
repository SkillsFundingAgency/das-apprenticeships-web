using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Domain;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Responses;
using SFA.DAS.Apprenticeships.Domain.Interfaces;
using SFA.DAS.Apprenticeships.Web.Extensions;
using SFA.DAS.Apprenticeships.Web.Infrastructure;
using SFA.DAS.Apprenticeships.Web.Models;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfPrice;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfStartDate;
using SFA.DAS.Employer.Shared.UI;

namespace SFA.DAS.Apprenticeships.Web.Controllers.ChangeOfStartDate;

public class ChangeOfStartDateEmployerController : Controller
{
    private readonly ILogger<ChangeOfStartDateEmployerController> _logger;
    private readonly IApprenticeshipService _apprenticeshipService;
    private readonly IMapper _mapper;
    private readonly UrlBuilder _externalEmployerUrlHelper;


    public const string ApproveProviderChangeOfStartDateViewName = "~/Views/ChangeOfStartDate/Employer/ApproveProviderChangeOfStartDate.cshtml";

    public ChangeOfStartDateEmployerController(
        ILogger<ChangeOfStartDateEmployerController> logger, 
        IApprenticeshipService apprenticeshipService,
        IMapper mapper,
        UrlBuilder externalEmployerUrlHelper)
    {
        _logger = logger;
        _apprenticeshipService = apprenticeshipService;
        _mapper = mapper;
        _externalEmployerUrlHelper = externalEmployerUrlHelper;
    }

    [HttpGet]
    [Authorize(Policy = nameof(PolicyNames.HasEmployerAccount))]
    [Route("employer/{employerAccountId}/ChangeOfStartDate/{apprenticeshipHashedId}/pending")]
    public async Task<IActionResult> ViewPendingChangePage(string employerAccountId, string apprenticeshipHashedId)
    {
        var response = await GetPendingStartDateChange(apprenticeshipHashedId);
        if (response == null)
        {
            return NotFound();
        }

        var backLink = _externalEmployerUrlHelper.CommitmentsV2Link("ApprenticeDetails", employerAccountId, apprenticeshipHashedId.ToUpper());

        switch (response.PendingStartDateChange!.Initiator.GetChangeInitiator())
        {
            case ChangeInitiator.Employer:
                throw new NotImplementedException("Employer initiated change of start date is not yet implemented");

            case ChangeInitiator.Provider:
                var providerInitiateViewModel = _mapper.Map<EmployerViewPendingStartDateChangeModel>(response);
                HttpContext.PopulateEmployerInitiatedRouteValues(providerInitiateViewModel);
                providerInitiateViewModel.BackLinkUrl = backLink;
                return View(ApproveProviderChangeOfStartDateViewName, providerInitiateViewModel);

        }

        throw new ArgumentOutOfRangeException("Unrecognised ChangeInitiator");
    }

    [HttpPost]
    [Authorize(Policy = nameof(PolicyNames.HasEmployerAccount))]
    [Route("employer/{employerAccountId}/ChangeOfStartDate/{apprenticeshipHashedId}/pending")]
    public async Task<IActionResult> ApproveOrRejectStartDateChange(string ApproveChanges, string rejectReason)
    {
        throw new NotImplementedException("Employer initiated change of start date is not yet implemented");
    }

    private async Task<GetPendingStartDateChangeResponse?> GetPendingStartDateChange(string apprenticeshipHashedId)
    {
        var apprenticeshipKey = await _apprenticeshipService.GetApprenticeshipKey(apprenticeshipHashedId);
        if (apprenticeshipKey == default)
        {
            _logger.LogWarning($"Apprenticeship key not found for apprenticeship with hashed id {apprenticeshipHashedId}");
            return null;
        }

        var pendingStartDateChange = await _apprenticeshipService.GetPendingStartDateChange(apprenticeshipKey);
        if (pendingStartDateChange == null || !pendingStartDateChange.HasPendingStartDateChange)
        {
            _logger.LogWarning($"Pending Apprenticeship Start Date not found for apprenticeshipKey {apprenticeshipKey}");
            return null;
        }

        return pendingStartDateChange;
    }

}
