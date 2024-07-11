using SFA.DAS.Apprenticeships.Web.Attributes;

namespace SFA.DAS.Apprenticeships.Web.Models;

public interface IApproveRequest
{
    [RadioOption]
    public string? ApproveRequest { get; set; }

    public string? RejectReason { get; set; }
}
