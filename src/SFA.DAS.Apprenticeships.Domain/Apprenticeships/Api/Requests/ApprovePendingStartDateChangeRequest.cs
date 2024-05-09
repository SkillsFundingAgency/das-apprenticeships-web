using SFA.DAS.Apprenticeships.Domain.Interfaces;

namespace SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Requests;

public class ApprovePendingStartDateChangeRequest : IPatchApiRequest
{
	private readonly Guid _apprenticeshipKey;

	public ApprovePendingStartDateChangeRequest(Guid apprenticeshipKey, ApprovePendingPriceChangeData data)
	{
		_apprenticeshipKey = apprenticeshipKey;
		Data = data;
	}

	public string PatchUrl => $"Apprenticeship/{_apprenticeshipKey}/startDateChange/pending/approve";
	public object Data { get; set; }
	public bool SendBearerToken => true;
}

public class ApprovePendingStartDateChangeData
{
#pragma warning disable CS8618 // Non-nullable field is uninitialized. This will be set when constructed
	public string UserId { get; set; }
#pragma warning restore CS8618
}