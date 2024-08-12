using SFA.DAS.Apprenticeships.Domain.Interfaces;

namespace SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Requests;

public class RejectPendingStartDateChangeRequest : IPatchApiRequest
{
	private readonly Guid _apprenticeshipKey;

	public RejectPendingStartDateChangeRequest(Guid apprenticeshipKey, RejectPendingStartDateChangeData data)
	{
		_apprenticeshipKey = apprenticeshipKey;
		Data = data;
	}

	public string PatchUrl => $"Apprenticeship/{_apprenticeshipKey}/startDateChange/pending/reject";
	public object Data { get; set; }
	public bool SendBearerToken => true;
}

public class RejectPendingStartDateChangeData
{
#pragma warning disable CS8618 // Non-nullable field is uninitialized. This will be set when constructed
	public string Reason { get; set; }
#pragma warning restore CS8618
}