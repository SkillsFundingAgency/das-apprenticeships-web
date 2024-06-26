﻿namespace SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;

public class PendingStartDateChange
{
    public DateTime? OriginalActualStartDate { get; set; }
    public DateTime? PendingActualStartDate { get; set; }
    public string? Reason { get; set; }
    public long? Ukprn { get; set; }
    public long? AccountLegalEntityId { get; set; }
    public Guid ApprenticeshipKey { get; set; }
    public DateTime? ProviderApprovedDate { get; set; }
    public DateTime? EmployerApprovedDate { get; set; }
    public string? Initiator { get; set; }
}
