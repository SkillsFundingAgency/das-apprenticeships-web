using SFA.DAS.Apprenticeships.Domain.Interfaces;

namespace SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Requests;

public class CancelPendingStartDateChangeRequest : IDeleteApiRequest
{
	private readonly Guid _apprenticeshipKey;

	public CancelPendingStartDateChangeRequest(Guid apprenticeshipKey)
	{
		_apprenticeshipKey = apprenticeshipKey;
	}

	public string DeleteUrl => $"Apprenticeship/{_apprenticeshipKey}/startDateChange/pending";
	public bool SendBearerToken => true;
}