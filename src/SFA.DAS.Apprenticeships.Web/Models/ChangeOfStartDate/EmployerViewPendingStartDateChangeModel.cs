using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Responses;
using SFA.DAS.Apprenticeships.Web.Attributes;
using SFA.DAS.Apprenticeships.Web.Exceptions;
using SFA.DAS.Apprenticeships.Web.Extensions;
using System.Web;

namespace SFA.DAS.Apprenticeships.Web.Models.ChangeOfStartDate;

public class EmployerViewPendingStartDateChangeModel : BaseChangeOfStartDateModel, IRouteValuesEmployer, IApproveRequest, ICacheModel
{
    public string EmployerAccountId { get; set; } = string.Empty;
    public string BackLinkUrl { get; set; } = string.Empty;
    public string ProviderName { get; set; } = string.Empty;
    public DateTime OriginalActualStartDate { get; set; }
    public DateTime PendingActualStartDate { get; set; }
    public DateTime OriginalPlannedEndDate { get; set; }
    public DateTime PendingPlannedEndDate { get; set; }
    [RadioOption]
    public string? ApproveRequest { get; set; }
    public string? RejectReason { get; set; }
}

public class EmployerViewPendingStartDateChangeModelMapper : IMapper<EmployerViewPendingStartDateChangeModel>
{
    public EmployerViewPendingStartDateChangeModel Map(object sourceObject)
    {
        if (sourceObject is GetPendingStartDateChangeResponse getPendingStartDateChangeResponse)
        {
            return FromGetPendingStartDateChangeResponse(getPendingStartDateChangeResponse);
        }

        throw new NotImplementedException($"There is not mapping available for object of type {sourceObject.GetType().Name}");
    }

    private static EmployerViewPendingStartDateChangeModel FromGetPendingStartDateChangeResponse(GetPendingStartDateChangeResponse getPendingStartDateChangeResponse)
    {
        var pendingStartDateChange = getPendingStartDateChangeResponse.PendingStartDateChange;

        if (pendingStartDateChange == null)
        {
            throw new MapperException($"Could not map {nameof(GetPendingStartDateChangeResponse)} to {nameof(EmployerViewPendingStartDateChangeModel)} as {nameof(GetPendingStartDateChangeResponse)}.{nameof(PendingStartDateChange)} is null");
        }

        var model = new EmployerViewPendingStartDateChangeModel
        {
            ApprenticeshipKey = pendingStartDateChange.ApprenticeshipKey,
            ReasonForChangeOfStartDate = HttpUtility.HtmlDecode(pendingStartDateChange.Reason),
            ProviderName = getPendingStartDateChangeResponse.ProviderName.ValueOrSubstitute("The Provider"),
            OriginalActualStartDate = pendingStartDateChange.OriginalActualStartDate.ValueOrThrow(nameof(PendingStartDateChange.OriginalActualStartDate)),
            PendingActualStartDate = pendingStartDateChange.PendingActualStartDate.ValueOrThrow(nameof(PendingStartDateChange.PendingActualStartDate)),
            OriginalPlannedEndDate = pendingStartDateChange.OriginalPlannedEndDate.ValueOrThrow(nameof(PendingStartDateChange.OriginalPlannedEndDate)),
            PendingPlannedEndDate = pendingStartDateChange.PendingPlannedEndDate.ValueOrThrow(nameof(PendingStartDateChange.PendingPlannedEndDate))
        };

        return model;
    }
}