﻿#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;

public class PendingStartDateChange
{
    public DateTime? OriginalActualStartDate { get; set; }
    public DateTime? PendingActualStartDate { get; set; }
    public DateTime? OriginalPlannedEndDate { get; set; }
    public DateTime? PendingPlannedEndDate { get; set; }
    public string? Reason { get; set; }
    public long? Ukprn { get; set; }
    public long? AccountLegalEntityId { get; set; }
    public Guid ApprenticeshipKey { get; set; }
    public DateTime? ProviderApprovedDate { get; set; }
    public DateTime? EmployerApprovedDate { get; set; }
    public string? Initiator { get; set; }
}
