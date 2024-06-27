namespace SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Responses;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public class GetPendingPriceChangeResponse
{
    public bool HasPendingPriceChange { get; set; }
    public PendingPriceChange PendingPriceChange { get; set; }
    public string? ProviderName { get; set; }
    public string? EmployerName { get; set; }
}