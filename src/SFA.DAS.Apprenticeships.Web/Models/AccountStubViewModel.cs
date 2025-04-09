using System.Diagnostics.CodeAnalysis;
using SFA.DAS.Apprenticeships.Domain.Employers;
using SFA.DAS.GovUK.Auth.Employer;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace SFA.DAS.Apprenticeships.Web.Models;

[ExcludeFromCodeCoverage]
public class AccountStubViewModel
{
    public string Id { get; set; }
    public string Email { get; set; }
    public List<EmployerUserAccountItem> Accounts { get; set; }
    public string ReturnUrl { get; set; }
}