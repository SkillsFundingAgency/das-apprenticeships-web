using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Apprenticeships.Web.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public static class EmployerClaims
    {
        public static string AccountsClaimsTypeIdentifier => "http://das/employer/identity/claims/associatedAccounts";
        public static string EmployerEmailClaimsTypeIdentifier => "http://das/employer/identity/claims/email_address";
        public static string IdamsUserIdClaimTypeIdentifier => "http://das/employer/identity/claims/id";
        public static string IdamsUserDisplayNameClaimTypeIdentifier = "http://das/employer/identity/claims/display_name";
        public static string Account => "http://das/employer/identity/claims/account";
        public const string GivenName = "http://das/employer/identity/claims/given_name";
        public const string FamilyName = "http://das/employer/identity/claims/family_name";
    }
}