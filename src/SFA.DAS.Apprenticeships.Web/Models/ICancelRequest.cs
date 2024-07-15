using SFA.DAS.Apprenticeships.Web.Attributes;

namespace SFA.DAS.Apprenticeships.Web.Models;

public interface ICancelRequest
{
    [RadioOption]
    public string? CancelRequest { get; set; }
}
