using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SFA.DAS.Apprenticeships.Domain.Employers;
using SFA.DAS.GovUK.Auth.Employer;
using EmployerClaims = SFA.DAS.Apprenticeships.Web.Infrastructure.EmployerClaims;

namespace SFA.DAS.Apprenticeships.Web.Identity.Authentication;

[ExcludeFromCodeCoverage]
public class EmployerStubAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{

    public EmployerStubAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var accountClaims = new Dictionary<string, EmployerUserAccountItem>();
        accountClaims.Add("", new EmployerUserAccountItem
        {
            Role = "Owner",
            AccountId = "ABC123",
            EmployerName = "Stub Employer",
            ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy
        });

        var claims = new[]
        {
            new Claim(EmployerClaims.AccountsClaimsTypeIdentifier, JsonConvert.SerializeObject(accountClaims)),
            new Claim(EmployerClaims.EmployerEmailClaimsTypeIdentifier, "testemployer@user.com"),
        };
        var identity = new ClaimsIdentity(claims, "Employer-stub");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Employer-stub");

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }
}