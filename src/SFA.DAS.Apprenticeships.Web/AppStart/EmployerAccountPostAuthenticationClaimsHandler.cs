using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SFA.DAS.Apprenticeships.Domain.Employers;
using SFA.DAS.Apprenticeships.Domain.Interfaces;
using SFA.DAS.Apprenticeships.Infrastructure.Configuration;
using SFA.DAS.Apprenticeships.Web.Infrastructure;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.Apprenticeships.Web.AppStart;

public class EmployerAccountPostAuthenticationClaimsHandler : ICustomClaims
{
    private readonly IEmployerAccountService _accountsSvc;
    private readonly IConfiguration _configuration;
    private readonly ApprenticeshipsWeb _apprenticeshipsWebConfiguration;

    public EmployerAccountPostAuthenticationClaimsHandler(IEmployerAccountService accountsSvc, IConfiguration configuration, IOptions<ApprenticeshipsWeb> apprenticeshipsWebConfiguration)
    {
        _accountsSvc = accountsSvc;
        _configuration = configuration;
        _apprenticeshipsWebConfiguration = apprenticeshipsWebConfiguration.Value;
    }
    public async Task<IEnumerable<Claim>> GetClaims(TokenValidatedContext ctx)
    {
        var claims = new List<Claim>()
        {
            new(UserClaims.AuthenticationType, AuthenticationType.Employer.ToString())
        };

        if (_configuration.UseLocalStubAuth())
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

        var userId = ctx.Principal.Claims
            .First(c => c.Type.Equals(ClaimTypes.NameIdentifier))
            .Value;
        var email = ctx.Principal.Claims
            .First(c => c.Type.Equals(ClaimTypes.Email))
            .Value;
        
        var returnClaims = new List<Claim>();
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