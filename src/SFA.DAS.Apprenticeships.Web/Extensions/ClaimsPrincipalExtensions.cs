using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using SFA.DAS.Apprenticeships.Web.Infrastructure;

namespace SFA.DAS.Apprenticeships.Web.Extensions;

[ExcludeFromCodeCoverage]
public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal user)
    {
        return user.FindFirst(EmployerClaims.IdamsUserIdClaimTypeIdentifier)?.Value;
    }
}