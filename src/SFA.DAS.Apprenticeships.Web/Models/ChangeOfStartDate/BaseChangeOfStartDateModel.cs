﻿namespace SFA.DAS.Apprenticeships.Web.Models.ChangeOfStartDate;

public class BaseChangeOfStartDateModel
{
    public string? CacheKey { get; set; }
    public Guid ApprenticeshipKey { get; set; }
    public string? ApprenticeshipHashedId { get; set; }
    public string? ReasonForChangeOfStartDate { get; set; }
    public string? ApprovingPartyName { get; set; }
}

public class BaseProviderChangeOfStartDateModel : BaseChangeOfStartDateModel
{
    public long? ProviderReferenceNumber { get; set; }
}
