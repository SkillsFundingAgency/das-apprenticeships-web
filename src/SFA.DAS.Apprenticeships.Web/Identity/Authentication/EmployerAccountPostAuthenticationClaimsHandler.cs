using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Newtonsoft.Json;
using SFA.DAS.Apprenticeships.Domain.Interfaces;
using SFA.DAS.Apprenticeships.Web.Infrastructure;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.Apprenticeships.Web.Identity.Authentication;

[ExcludeFromCodeCoverage]
public class EmployerAccountPostAuthenticationClaimsHandler : ICustomClaims
{
    private readonly IEmployerAccountService _accountsSvc;

    public EmployerAccountPostAuthenticationClaimsHandler(IEmployerAccountService accountsSvc)
    {
        _accountsSvc = accountsSvc;
    }

    public async Task<IEnumerable<Claim>> GetClaims(TokenValidatedContext tokenValidatedContext)
    {
        var claims = new List<Claim>();
        if (tokenValidatedContext.Principal == null) throw new ArgumentNullException(nameof(tokenValidatedContext), "ClaimsPrincipal is null");
        var userId = tokenValidatedContext.Principal.Claims
            .First(c => c.Type.Equals(ClaimTypes.NameIdentifier))
            .Value;
        var email = tokenValidatedContext.Principal.Claims
            .First(c => c.Type.Equals(ClaimTypes.Email))
            .Value;

        var result = await _accountsSvc.GetUserAccounts(userId, email);

        var accountsAsJson = JsonConvert.SerializeObject(result.EmployerAccounts.ToDictionary(k => k.AccountId));
        var associatedAccountsClaim = new Claim(EmployerClaims.AccountsClaimsTypeIdentifier, accountsAsJson, JsonClaimValueTypes.Json);

        if (result.IsSuspended)
        {
            claims.Add(new Claim(ClaimTypes.AuthorizationDecision, "Suspended"));
        }
        if (!string.IsNullOrEmpty(result.FirstName) && !string.IsNullOrEmpty(result.LastName))
        {
            claims.Add(new Claim(EmployerClaims.GivenName, result.FirstName));
            claims.Add(new Claim(EmployerClaims.FamilyName, result.LastName));
            claims.Add(new Claim(EmployerClaims.IdamsUserDisplayNameClaimTypeIdentifier, result.FirstName + " " + result.LastName));
        }

        claims.Add(new Claim(EmployerClaims.IdamsUserIdClaimTypeIdentifier, result.EmployerUserId));
        claims.Add(new Claim(EmployerClaims.EmployerEmailClaimsTypeIdentifier, email));

        result.EmployerAccounts
            .Where(c => c.Role.Equals("owner", StringComparison.CurrentCultureIgnoreCase) || c.Role.Equals("transactor", StringComparison.CurrentCultureIgnoreCase))
            .ToList().ForEach(u => claims.Add(new Claim(EmployerClaims.Account, u.AccountId)));

        claims.Add(associatedAccountsClaim);

        return claims;
    }
}