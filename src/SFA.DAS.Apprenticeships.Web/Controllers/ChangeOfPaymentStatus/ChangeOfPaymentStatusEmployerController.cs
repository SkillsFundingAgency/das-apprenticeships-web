using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Domain.Interfaces;
using SFA.DAS.Apprenticeships.Web.Constants.Employer;
using SFA.DAS.Apprenticeships.Web.Extensions;
using SFA.DAS.Apprenticeships.Web.Infrastructure;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfPaymentStatus;
using SFA.DAS.Apprenticeships.Web.Models.Enums;

namespace SFA.DAS.Apprenticeships.Web.Controllers.ChangeOfPaymentStatus;

[Authorize(Policy = nameof(PolicyNames.HasEmployerAccount))]
[Route("employer/{employerAccountId}/PaymentsFreeze/{apprenticeshipHashedId}")]
public class ChangeOfPaymentStatusEmployerController : Controller
{
    private readonly ILogger<ChangeOfPaymentStatusEmployerController> _logger;
    private readonly IApprenticeshipService _apprenticeshipService;
    private readonly UrlBuilder _externalEmployerUrlHelper;

    public const string FreezeProviderPaymentsViewName = "~/Views/ChangeOfPaymentStatus/FreezeProviderPayments.cshtml";

    public ChangeOfPaymentStatusEmployerController(ILogger<ChangeOfPaymentStatusEmployerController> logger, IApprenticeshipService apprenticeshipService, UrlBuilder externalEmployerUrlHelper)
    {
        _logger = logger;
        _apprenticeshipService = apprenticeshipService;
        _externalEmployerUrlHelper = externalEmployerUrlHelper;
    }

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> FreezeProviderPaymentsPage(string employerAccountId, string apprenticeshipHashedId)
    {
        var response = await _apprenticeshipService.GetApprenticeshipKey(apprenticeshipHashedId);
        if (response == Guid.Empty)
        {
            return NotFound();
        }

        var backLink = _externalEmployerUrlHelper.CommitmentsV2Link(EmployerRoutes.ApprenticeDetails, employerAccountId, apprenticeshipHashedId.ToUpper());

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

            var redirectUrl = _externalEmployerUrlHelper
                .CommitmentsV2Link(EmployerRoutes.ApprenticeDetails, model.EmployerAccountId, model.ApprenticeshipHashedId?.ToUpper())
                .AppendEmployerBannersToUrl(EmployerApprenticeDetailsBanners.ProviderPaymentsInactive);
            return Redirect(redirectUrl);
        }

        return Redirect(_externalEmployerUrlHelper.CommitmentsV2Link(EmployerRoutes.ApprenticeDetails, model.EmployerAccountId, model.ApprenticeshipHashedId?.ToUpper()));
    }
}