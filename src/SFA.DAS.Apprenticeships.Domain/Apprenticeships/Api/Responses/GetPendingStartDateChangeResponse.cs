namespace SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Responses;

public class GetPendingStartDateChangeResponse
{
    public bool HasPendingStartDateChange { get; set; }
    public PendingStartDateChange? PendingStartDateChange { get; set; }
    public string? ProviderName { get; set; }
    public string? EmployerName { get; set; }
}
