using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Apprenticeships.Web.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public static class PolicyNames
    {
        public static string HasEmployerAccount => nameof(HasEmployerAccount);
        public static string HasProviderAccount => nameof(HasProviderAccount);
        public static string IsAuthenticated => nameof(IsAuthenticated);
    }
}