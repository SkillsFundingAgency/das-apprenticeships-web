using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Domain;
using SFA.DAS.Apprenticeships.Web.Extensions;
using SFA.DAS.Apprenticeships.Web.Infrastructure;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfStartDate;
using SFA.DAS.Apprenticeships.Web.Models;
using SFA.DAS.Apprenticeships.Domain.Interfaces;
using SFA.DAS.Apprenticeships.Web.Controllers.ChangeOfPrice;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.Apprenticeships.Web.Models.PaymentsFreeze;

namespace SFA.DAS.Apprenticeships.Web.Controllers.PaymentsFreeze
{
	//[Authorize]
	[Route("employer/{employerAccountId}/PaymentsFreeze/{apprenticeshipHashedId}")]
	public class EmployerPaymentsFreezeController : Controller
	{
		private readonly ILogger<EmployerPaymentsFreezeController> _logger;
		private readonly IApprenticeshipService _apprenticeshipService;
		private readonly UrlBuilder _externalEmployerUrlHelper;

        public const string FreezeProviderPaymentsViewName = "~/Views/PaymentsFreeze/FreezeProviderPayments.cshtml";

        public EmployerPaymentsFreezeController(ILogger<EmployerPaymentsFreezeController> logger, IApprenticeshipService apprenticeshipService, UrlBuilder externalEmployerUrlHelper)
		{
			_logger = logger;
			_apprenticeshipService = apprenticeshipService;
			_externalEmployerUrlHelper = externalEmployerUrlHelper;
		}

		[HttpGet]
		//[Authorize(Policy = nameof(PolicyNames.HasEmployerAccount))]
		[Route("")]
		public async Task<IActionResult> FreezeProviderPaymentsPage(string employerAccountId, string apprenticeshipHashedId)
		{
			//var response = await _apprenticeshipService.GetApprenticeshipKey(apprenticeshipHashedId);
			//if (response == Guid.Empty)
			//{
			//	return NotFound();
			//}

            var response = new Guid("FB25B5EC-7603-48F2-B54F-E16A73E024C5");

			var backLink = _externalEmployerUrlHelper.CommitmentsV2Link("ApprenticeDetails", employerAccountId, apprenticeshipHashedId.ToUpper());

            var model = new FreezeProviderPaymentsModel{ ApprenticeshipKey = response, ApprenticeshipHashedId = apprenticeshipHashedId, BackLinkUrl = backLink, EmployerAccountId = employerAccountId };

            return View(FreezeProviderPaymentsViewName, model);
        }

        [HttpPost]
        //[Authorize(Policy = nameof(PolicyNames.HasEmployerAccount))]
        [Route("")]
        public async Task<IActionResult> FreezeProviderPaymentsPage(string employerAccountId, string apprenticeshipHashedId, bool FreezePayments, string reasonForFreeze, FreezeProviderPaymentsModel model)
        {
            //todo do freeze

            var response = new Guid("FB25B5EC-7603-48F2-B54F-E16A73E024C5");

            var backLink = _externalEmployerUrlHelper.CommitmentsV2Link("ApprenticeDetails", employerAccountId, apprenticeshipHashedId.ToUpper());

            //var model = new FreezeProviderPaymentsModel { ApprenticeshipKey = response, ApprenticeshipHashedId = apprenticeshipHashedId, BackLinkUrl = backLink, EmployerAccountId = employerAccountId };

            return View(FreezeProviderPaymentsViewName, model);
        }
    }
}
