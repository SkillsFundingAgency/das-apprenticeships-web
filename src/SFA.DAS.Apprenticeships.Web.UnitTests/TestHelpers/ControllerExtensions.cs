using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.TestHelpers
{
    public static class ControllerExtensions
    {
        public static void SetUserName(this ControllerBase controller, string userName)
        {
            var contextMock = new Mock<HttpContext>();
            var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
            claimsPrincipalMock.Setup(x => x.Identity!.Name).Returns(userName);
            contextMock.Setup(ctx => ctx.User).Returns(claimsPrincipalMock.Object);
            controller.ControllerContext.HttpContext = contextMock.Object;
        }
    }
}
