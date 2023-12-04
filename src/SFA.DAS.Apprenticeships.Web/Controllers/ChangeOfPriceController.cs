using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Domain.Interfaces;
using SFA.DAS.Apprenticeships.Web.Extensions;
using SFA.DAS.Apprenticeships.Web.Infrastructure;
using SFA.DAS.Apprenticeships.Web.Models;
using SFA.DAS.Provider.Shared.UI;
using SFA.DAS.Provider.Shared.UI.Attributes;

namespace SFA.DAS.Apprenticeships.Web.Controllers
{
    public class ChangeOfPriceController : Controller
    {
        private readonly ILogger<ChangeOfPriceController> _logger;
        private readonly IApprenticeshipService _apprenticeshipService;
        private readonly IMapper<CreateChangeOfPriceModel> _mapper;
        public const string ProviderInitiatedViewName = "ProviderInitiated";

        public ChangeOfPriceController(ILogger<ChangeOfPriceController> logger, IApprenticeshipService apprenticeshipService, IMapper<CreateChangeOfPriceModel> mapper)
        {
            _logger = logger;
            _apprenticeshipService = apprenticeshipService;
            _mapper = mapper;
        }

        [HttpGet]
        [SetNavigationSection(NavigationSection.ManageApprentices)]
        [Route("provider/{ukprn}/ChangeOfPrice/{apprenticeshipHashedId}")]
        public async Task<IActionResult> GetProviderInitiatedPage(string apprenticeshipHashedId)
        {
            var apprenticeshipKey = await _apprenticeshipService.GetApprenticeshipKey(apprenticeshipHashedId);
            if(apprenticeshipKey == default(Guid))
            {
                _logger.LogWarning($"Apprenticeship key not found for apprenticeship with hashed id {apprenticeshipHashedId}");
                return NotFound();
            }

            var apprenticeshipPrice = await _apprenticeshipService.GetApprenticeshipPrice(apprenticeshipKey);
            if (apprenticeshipPrice == null || apprenticeshipPrice.ApprenticeshipKey != apprenticeshipKey)
            {
                _logger.LogWarning($"ApprenticeshipPrice not found for apprenticeshipKey {apprenticeshipKey}");
                return NotFound();
            }

            var model = _mapper.Map(apprenticeshipPrice);
            model.ApprenticeshipKey = apprenticeshipKey;
            PopulateProviderInitiatedRouteValues(model);
            return View(ProviderInitiatedViewName, model);
        }

        [HttpPost]
        [SetNavigationSection(NavigationSection.ManageApprentices)]
        [Route("provider/{ukprn}/ChangeOfPrice/{apprenticeshipHashedId}")]
        public async Task<IActionResult> ProviderInitiatedPriceChangeRequest(CreateChangeOfPriceModel model, long ukprn)
        {
            if (!ModelState.IsValid)
            {
                PopulateProviderInitiatedRouteValues(model);
                return View(ProviderInitiatedViewName, model);
            }

            await _apprenticeshipService.CreatePriceHistory(model.ApprenticeshipKey, ukprn, null, "todo FLP-473", model.ApprenticeshipTrainingPrice, model.ApprenticeshipEndPointAssessmentPrice, model.ApprenticeshipTotalPrice, "todo FLP-354", model.EffectiveFromDate.Date.GetValueOrDefault());
            throw new NotImplementedException("todo link back to provider commitments");
        }

        //  If other endpoints use the same route values, this could be refactored to take an interface/abstract class instead of CreateChangeOfPriceModel
        private void PopulateProviderInitiatedRouteValues(CreateChangeOfPriceModel model)
        {
            model.ApprenticeshipHashedId = HttpContext.GetRouteValueString(RouteValues.ApprenticeshipHashedId);
            model.ProviderReferenceNumber = HttpContext.GetRouteValueString(RouteValues.Ukprn);
        }
    }
}