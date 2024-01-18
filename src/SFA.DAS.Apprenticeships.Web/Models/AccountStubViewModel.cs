using System.Diagnostics.CodeAnalysis;
using SFA.DAS.Apprenticeships.Domain.Employers;

namespace SFA.DAS.Apprenticeships.Web.Models;

[ExcludeFromCodeCoverage]
public class AccountStubViewModel
{
    public string Id { get; set; }
    public string Email { get; set; }
    public List<EmployerUserAccountItem> Accounts { get; set; }
    public string ReturnUrl { get; set; }
}