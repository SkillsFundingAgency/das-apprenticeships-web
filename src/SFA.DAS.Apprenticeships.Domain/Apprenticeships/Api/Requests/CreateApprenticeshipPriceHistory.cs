using SFA.DAS.Apprenticeships.Domain.Interfaces;

namespace SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Requests;

public class CreateApprenticeshipPriceHistoryRequest : IPostApiRequest
{
    private readonly Guid _apprenticeshipKey;

    public CreateApprenticeshipPriceHistoryRequest(Guid apprenticeshipKey, CreateApprenticeshipPriceHistoryData data)
    {
        _apprenticeshipKey = apprenticeshipKey;
        Data = data;
    }

    public string PostUrl => $"Apprenticeship/{_apprenticeshipKey}/priceHistory";
    public object Data { get; set; }
}

public class CreateApprenticeshipPriceHistoryData
{
    public string Initiator { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public decimal? TrainingPrice { get; set; }
    public decimal? AssessmentPrice { get; set; }
    public decimal? TotalPrice { get; set; }
    public string? Reason { get; set; }
    public DateTime EffectiveFromDate { get; set; }
}