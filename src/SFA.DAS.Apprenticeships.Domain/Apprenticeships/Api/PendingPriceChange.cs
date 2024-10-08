﻿#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;

public class PendingPriceChange
{
    public decimal? OriginalTrainingPrice { get; set; }
    public decimal? OriginalAssessmentPrice { get; set; }
    public decimal OriginalTotalPrice { get; set; }
    public decimal? PendingTrainingPrice { get; set; }
    public decimal? PendingAssessmentPrice { get; set; }
    public decimal PendingTotalPrice { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public string Reason { get; set; }
    public Guid ApprenticeshipKey { get; set; }
    public DateTime? ProviderApprovedDate { get; set; }
    public DateTime? EmployerApprovedDate { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Initiator { get; set; }
}