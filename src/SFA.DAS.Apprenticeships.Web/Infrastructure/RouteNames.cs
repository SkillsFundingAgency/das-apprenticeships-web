using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Apprenticeships.Web.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public static class RouteNames
    {
        public const string CreatePriceChangeRequest = "create-price-change-request";
        public const string SignOut = "sign-out";
        public const string SignedOut = "signed-out";
        public const string ProviderSignOut = "provider-signout";
    }
}