using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using SFA.DAS.Apprenticeships.Web.Infrastructure;
using System.Net.Http;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.TestHelpers;

public static class ControllerExtensions
{
    public static Dictionary<Type, object> SetupHttpContext(this ControllerBase controller, long? providerReferenceNumber = null, string? apprenticeshipHashedId = null, string? userName = null, string? employerAccountId = null)
    {
        var trackedMocks = new Dictionary<Type, object>();

        if (controller.HttpContext == null)
        {
            var httpContext = trackedMocks.CreateTrackedMock<HttpContext>();
            var httpRequest = trackedMocks.CreateTrackedMock<HttpRequest>();

            httpRequest.Setup(m => m.RouteValues).Returns(new RouteValueDictionary());
            httpContext.Setup(m => m.Request).Returns(httpRequest.Object);

            var claimsPrincipalMock = trackedMocks.CreateTrackedMock<ClaimsPrincipal>();

            if (userName != null)
            {
                claimsPrincipalMock.Setup(x => x.FindFirst(EmployerClaims.IdamsUserIdClaimTypeIdentifier)).Returns(new Claim(EmployerClaims.IdamsUserIdClaimTypeIdentifier, userName));
            }
                
            claimsPrincipalMock.Setup(x => x.Identity!.Name).Returns(userName);
            httpContext.Setup(ctx => ctx.User).Returns(claimsPrincipalMock.Object);

            controller.ControllerContext.HttpContext = httpContext.Object;
        }

        if (providerReferenceNumber.HasValue)
            controller.HttpContext!.Request.RouteValues.Add(RouteValues.Ukprn, providerReferenceNumber.ToString());

        if (employerAccountId != null)
            controller.HttpContext!.Request.RouteValues.Add(RouteValues.EmployerAccountId, employerAccountId);

        if(apprenticeshipHashedId != null)
            controller.HttpContext!.Request.RouteValues.Add(RouteValues.ApprenticeshipHashedId, apprenticeshipHashedId);

        return trackedMocks;
    }

    public static void SetQueryString(this Dictionary<Type, object> trackedMocks, KeyValuePair<string,string>[] queryParams)
    {
        var httpRequest = trackedMocks[typeof(HttpRequest)];
        if (httpRequest == null)
            throw new InvalidOperationException("HttpContext not setup");

        var mock = (Mock<HttpRequest>)httpRequest;

        var dictionary = queryParams.ToDictionary(x => x.Key, x => (Microsoft.Extensions.Primitives.StringValues)x.Value);
        mock.Setup(m => m.Query).Returns(new QueryCollection(dictionary));
    }

    private static Mock<T> CreateTrackedMock<T>(this Dictionary<Type, object> trackedMocks) where T: class
    {
        var mock = new Mock<T>();
        trackedMocks.Add(typeof(T), mock);
        return mock;
    }
}