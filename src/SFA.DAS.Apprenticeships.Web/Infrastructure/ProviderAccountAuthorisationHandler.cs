using Microsoft.AspNetCore.Authorization;

namespace SFA.DAS.Apprenticeships.Web.Infrastructure
{
    /// <summary>
    /// Authorisation handler that ensures that the UkPrn value of the authenticated Provider matches that of incoming requests.
    /// </summary>
    public class ProviderAccountAuthorisationHandler : AuthorizationHandler<ProviderAccountRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProviderAccountAuthorisationHandler (IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ProviderAccountRequirement requirement)
        {
            if (!IsProviderAuthorised(context)) 
            {
                context.Fail();
                return Task.CompletedTask;
            }
                
            context.Succeed(requirement);

            return Task.CompletedTask;
        }
        
        public bool IsProviderAuthorised(AuthorizationHandlerContext context)
        {
            if (!context.User.HasClaim(c => c.Type.Equals(ProviderClaims.ProviderUkprn)))
            {
                return false;
            }
            
            if (_httpContextAccessor.HttpContext.Request.RouteValues.ContainsKey(RouteValues.Ukprn))
            {
                var ukPrnFromUrl = _httpContextAccessor.HttpContext.Request.RouteValues[RouteValues.Ukprn].ToString();
                var ukPrn = context.User.FindFirst(c => c.Type.Equals(ProviderClaims.ProviderUkprn)).Value;

                return ukPrn.Equals(ukPrnFromUrl);    
            }

            return true;
        }
    }
}