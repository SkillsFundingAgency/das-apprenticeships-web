using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using SFA.DAS.Apprenticeships.Web.Infrastructure;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.TestHelpers
{
    public static class ControllerExtensions
    {
        public static void SetupHttpContext(this ControllerBase controller, long? providerReferenceNumber = null, string apprenticeshipHashedId = null, string userName = null)
        {
            if (controller.HttpContext == null)
            {
                var httpContext = new Mock<HttpContext>();

                var httpRequest = new Mock<HttpRequest>();
                httpRequest.Setup(m => m.RouteValues).Returns(new RouteValueDictionary());
                httpContext.Setup(m => m.Request).Returns(httpRequest.Object);

                var claimsPrincipalMock = new Mock<ClaimsPrincipal>();

                if (userName != null)
                {
                    claimsPrincipalMock.Setup(x => x.FindFirst(EmployerClaims.IdamsUserIdClaimTypeIdentifier)).Returns(new Claim(EmployerClaims.IdamsUserIdClaimTypeIdentifier, userName));
                }
                
                claimsPrincipalMock.Setup(x => x.Identity!.Name).Returns(userName);
                httpContext.Setup(ctx => ctx.User).Returns(claimsPrincipalMock.Object);

                controller.ControllerContext.HttpContext = httpContext.Object;
            }

            controller.HttpContext!.Request.RouteValues.Add(RouteValues.Ukprn, providerReferenceNumber.ToString());
            controller.HttpContext.Request.RouteValues.Add(RouteValues.ApprenticeshipHashedId, apprenticeshipHashedId);
        }
    }
}
