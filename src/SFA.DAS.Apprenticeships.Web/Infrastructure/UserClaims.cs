using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Apprenticeships.Web.Infrastructure;

[ExcludeFromCodeCoverage]
public static class UserClaims
{
    public static string AuthenticationType => "http://schemas.portal.com/authtype";
}