using Microsoft.AspNetCore.Authorization;

namespace SFA.DAS.Apprenticeships.Web.Infrastructure
{
    public interface IProviderAccountAuthorisationHandler
    {
        bool IsProviderAuthorised(AuthorizationHandlerContext context);
    }
    public class ProviderAccountAuthorizationHandler : AuthorizationHandler<ProviderAccountRequirement>, IProviderAccountAuthorisationHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProviderAccountAuthorizationHandler (IHttpContextAccessor httpContextAccessor)
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