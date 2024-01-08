using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Newtonsoft.Json;
using SFA.DAS.Apprenticeships.Domain.Employers;
using SFA.DAS.Apprenticeships.Domain.Interfaces;
using SFA.DAS.Apprenticeships.Web.AppStart;
using SFA.DAS.Apprenticeships.Web.Infrastructure;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.Apprenticeships.Web.Identity.Authentication;

[ExcludeFromCodeCoverage]
public class EmployerAccountPostAuthenticationClaimsHandler : ICustomClaims
{
    private readonly IEmployerAccountService _accountsSvc;
    private readonly IConfiguration _configuration;

    public EmployerAccountPostAuthenticationClaimsHandler(IEmployerAccountService accountsSvc, IConfiguration configuration)
    {
        _accountsSvc = accountsSvc;
        _configuration = configuration;
    }

    public async Task<IEnumerable<Claim>> GetClaims(TokenValidatedContext tokenValidatedContext)
    {
        var claims = new List<Claim>()
        {
            new(UserClaims.AuthenticationType, AuthenticationType.Employer.ToString())
        };

        if (_configuration.UseStubAuth())
        {
            var accountClaims = new Dictionary<string, EmployerUserAccountItem>();
            accountClaims.Add("", new EmployerUserAccountItem
            {
                Role = "Owner",
                AccountId = "ABC123",
                EmployerName = "Stub Employer"
            });
            claims.Add(new Claim(EmployerClaims.AccountsClaimsTypeIdentifier, JsonConvert.SerializeObject(accountClaims)));
            claims.Add(new Claim(EmployerClaims.EmployerEmailClaimsTypeIdentifier, _configuration["NoAuthEmail"]));

            return claims.ToList();
        }
        
        var returnClaims = new List<Claim>();
        if (tokenValidatedContext.Principal == null)
        {
	        return returnClaims;
        }

        var userId = tokenValidatedContext.Principal.Claims
	        .First(c => c.Type.Equals(ClaimTypes.NameIdentifier))
	        .Value;
        var email = tokenValidatedContext.Principal.Claims
	        .First(c => c.Type.Equals(ClaimTypes.Email))
	        .Value;
        
        var result = await _accountsSvc.GetUserAccounts(userId, email);

        if (result.IsSuspended)
        {
	        returnClaims.Add(new Claim(ClaimTypes.AuthorizationDecision, "Suspended"));
        }

        var accountsAsJson = JsonConvert.SerializeObject(result.EmployerAccounts.ToDictionary(k => k.AccountId));
        returnClaims.Add(new Claim(EmployerClaims.AccountsClaimsTypeIdentifier, accountsAsJson, JsonClaimValueTypes.Json));
        return returnClaims;
    }
}