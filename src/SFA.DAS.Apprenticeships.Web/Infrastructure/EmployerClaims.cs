using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Apprenticeships.Web.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public static class EmployerClaims
    {
        public static string AccountsClaimsTypeIdentifier => "http://das/employer/identity/claims/associatedAccounts";
        public static string EmployerEmailClaimsTypeIdentifier => "http://das/employer/identity/claims/email_address";
        public static string IdamsUserIdClaimTypeIdentifier => "http://das/employer/identity/claims/id";
    }
}