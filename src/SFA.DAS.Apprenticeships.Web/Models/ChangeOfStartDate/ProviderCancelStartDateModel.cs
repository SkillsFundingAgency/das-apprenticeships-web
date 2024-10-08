﻿using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Responses;
using SFA.DAS.Apprenticeships.Web.Attributes;
using SFA.DAS.Apprenticeships.Web.Exceptions;
using SFA.DAS.Apprenticeships.Web.Extensions;
using System.Web;

namespace SFA.DAS.Apprenticeships.Web.Models.ChangeOfStartDate;

public class ProviderCancelStartDateModel : BaseProviderChangeOfStartDateModel, IRouteValuesProvider, ICacheModel, ICancelRequest
{
	public DateTime OriginalStartDate { get; set; }
	public DateTime PendingStartDate { get; set; }
	public DateTime OriginalPlannedEndDate { get; set; }
	public DateTime PendingPlannedEndDate { get; set; }
    [RadioOption]
    public string? CancelRequest { get; set; }
}

public class ProviderCancelStartDateModelMapper : IMapper<ProviderCancelStartDateModel>
{
    public ProviderCancelStartDateModel Map(object sourceObject)
    {
        if (sourceObject is GetPendingStartDateChangeResponse getPendingStartDateChangeResponse)
        {
            return FromGetPendingStartDateChangeResponse(getPendingStartDateChangeResponse);
        }

        throw new NotImplementedException($"There is not mapping available for object of type {sourceObject.GetType().Name}");
    }

    private static ProviderCancelStartDateModel FromGetPendingStartDateChangeResponse(GetPendingStartDateChangeResponse getPendingStartDateChangeResponse)
    {
        var pendingStartDateChange = getPendingStartDateChangeResponse.PendingStartDateChange;

        if (pendingStartDateChange == null)
        {
            throw new MapperException($"Could not map {nameof(GetPendingStartDateChangeResponse)} to {nameof(EmployerViewPendingStartDateChangeModel)} as {nameof(GetPendingStartDateChangeResponse)}.{nameof(PendingStartDateChange)} is null");
        }

        var model = new ProviderCancelStartDateModel
        {
            ApprenticeshipKey = pendingStartDateChange.ApprenticeshipKey,
            ReasonForChangeOfStartDate = HttpUtility.HtmlDecode(pendingStartDateChange.Reason),
            OriginalStartDate = pendingStartDateChange.OriginalActualStartDate.ValueOrThrow(nameof(PendingStartDateChange.OriginalActualStartDate)),
            PendingStartDate = pendingStartDateChange.PendingActualStartDate.ValueOrThrow(nameof(PendingStartDateChange.PendingActualStartDate)),
			OriginalPlannedEndDate = pendingStartDateChange.OriginalPlannedEndDate.ValueOrThrow(nameof(PendingStartDateChange.OriginalPlannedEndDate)),
			PendingPlannedEndDate = pendingStartDateChange.PendingPlannedEndDate.ValueOrThrow(nameof(PendingStartDateChange.PendingPlannedEndDate))
		};

        return model;
    }
}