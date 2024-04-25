using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using SFA.DAS.Apprenticeships.Web.AppStart;
using SFA.DAS.Apprenticeships.Web.Infrastructure;

namespace SFA.DAS.Apprenticeships.Web.Identity.Authentication
{
    [ExcludeFromCodeCoverage]
    public class ProviderStubAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly StubProviderUserClaims _stubProviderUserClaims;


        public ProviderStubAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            StubProviderUserClaims stubProviderUserClaims, 
            ILoggerFactory logger, 
            UrlEncoder encoder, 
            ISystemClock clock, 
            IHttpContextAccessor httpContextAccessor) : base(options, logger, encoder, clock)
        {
            _httpContextAccessor = httpContextAccessor;
            _stubProviderUserClaims = stubProviderUserClaims;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = GetClaims();
            var identity = new ClaimsIdentity(claims, "Provider-stub");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Provider-stub");

            var result = AuthenticateResult.Success(ticket);

            _httpContextAccessor.HttpContext!.Items.Add(ClaimsIdentity.DefaultNameClaimType, claims.First(x=>x.Type == ClaimsIdentity.DefaultNameClaimType));
            _httpContextAccessor.HttpContext.Items.Add(ProviderClaims.DisplayName, claims.First(x => x.Type == ProviderClaims.DisplayName));

            return Task.FromResult(result);
        }

        private Claim[] GetClaims()
        {
            //  If no stub claims are available, use the default claims
            var claims = new[]
{
                new Claim(UserClaims.AuthenticationType, AuthenticationType.Employer.ToString()),
                new Claim(ClaimsIdentity.DefaultNameClaimType, _stubProviderUserClaims.Name ?? "10000001"),
                new Claim(ProviderClaims.DisplayName,  _stubProviderUserClaims.DisplayName ?? "AED User"),
                new Claim(ProviderClaims.Service, "DAA"),
                new Claim(ProviderClaims.ProviderUkprn,  _stubProviderUserClaims.ProviderUkprn ?? "10000001")
            };

            return claims;
        }
    }

    public class StubProviderUserClaims
    {
        public string ProviderUkprn { get; set; }
        public string DisplayName { get; set; }
        public string Name { get; set; }
    }
}