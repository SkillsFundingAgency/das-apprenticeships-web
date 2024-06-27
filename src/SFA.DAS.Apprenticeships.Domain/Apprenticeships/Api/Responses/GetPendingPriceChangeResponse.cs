namespace SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Responses;

public class GetPendingPriceChangeResponse
{
    public bool HasPendingPriceChange { get; set; }
    public PendingPriceChange PendingPriceChange { get; set; } = null!;
    public string? ProviderName { get; set; }
    public string? EmployerName { get; set; }
}
