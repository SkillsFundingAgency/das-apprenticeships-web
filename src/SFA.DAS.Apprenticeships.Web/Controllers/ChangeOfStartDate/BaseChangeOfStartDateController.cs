using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Responses;
using SFA.DAS.Apprenticeships.Domain.Interfaces;

namespace SFA.DAS.Apprenticeships.Web.Controllers.ChangeOfStartDate
{
    public abstract class BaseChangeOfStartDateController<T> : Controller
    {
        private readonly ILogger<T> _logger;
        private readonly IApprenticeshipService _apprenticeshipService;

        protected BaseChangeOfStartDateController(ILogger<T> logger, IApprenticeshipService apprenticeshipService)
        {
            _logger = logger;
            _apprenticeshipService = apprenticeshipService;
        }

        protected async Task<GetPendingStartDateChangeResponse?> GetPendingStartDateChange(string apprenticeshipHashedId)
        {
            var apprenticeshipKey = await _apprenticeshipService.GetApprenticeshipKey(apprenticeshipHashedId);
            if (apprenticeshipKey == default)
            {
                _logger.LogWarning("Apprenticeship key not found for apprenticeship with hashed id {apprenticeshipHashedId}", apprenticeshipHashedId);
                return null;
            }

            var pendingStartDateChange = await _apprenticeshipService.GetPendingStartDateChange(apprenticeshipKey);
            if (pendingStartDateChange == null || !pendingStartDateChange.HasPendingStartDateChange)
            {
                _logger.LogWarning("Pending start date not found for apprenticeshipKey {apprenticeshipKey}", apprenticeshipKey);
                return null;
            }

            return pendingStartDateChange;
        }

		protected async Task<ApprenticeshipStartDate?> GetApprenticeshipStartDate(string apprenticeshipHashedId)
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
}
