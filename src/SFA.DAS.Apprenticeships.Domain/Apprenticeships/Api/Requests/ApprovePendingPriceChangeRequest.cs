using SFA.DAS.Apprenticeships.Domain.Interfaces;

namespace SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Requests;

public class ApprovePendingPriceChangeRequest : IPatchApiRequest
{
	private readonly Guid _apprenticeshipKey;

	public ApprovePendingPriceChangeRequest(Guid apprenticeshipKey, ApprovePendingPriceChangeData data)
	{
		_apprenticeshipKey = apprenticeshipKey;
		Data = data;
	}

	public string PatchUrl => $"Apprenticeship/{_apprenticeshipKey}/priceHistory/pending/approve";
	public object Data { get; set; }
}

public class ApprovePendingPriceChangeData
{
#pragma warning disable CS8618 // Non-nullable field is uninitialized. This will be set when constructed
	public string UserId { get; set; }
    public decimal? TrainingPrice { get; set; }
    public decimal? AssessmentPrice { get; set; }
#pragma warning restore CS8618
}