using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Moq;
using SFA.DAS.Apprenticeships.Web.Extensions;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Extensions
{
    public class HttpContextExtensionTests
    {
        [Test]
        public void GetRouteValueString_ReturnsExpectedValue()
        {
            // Arrange
            const string valueKey = "anyValueKey";
            const string routeValue = "anyRouteValue";

            var mockHttpContext = new Mock<HttpContext>();
            var mockHttpRequest = new Mock<HttpRequest>();
            mockHttpRequest.Setup(m => m.RouteValues).Returns(new RouteValueDictionary());
            mockHttpContext.Setup(m => m.Request).Returns(mockHttpRequest.Object);
            mockHttpContext.Object.Request.RouteValues.Add(valueKey, routeValue);

            // Act
            var result = mockHttpContext.Object.GetRouteValueString(valueKey);

            // Assert
            result.Should().Be(routeValue);
        }

        [Test]
        public void GetRouteValueString_ThrowsArgumentException()
        {
            // Arrange
            const string valueKey = "anyValueKey";

            var mockHttpContext = new Mock<HttpContext>();
            var mockHttpRequest = new Mock<HttpRequest>();
            mockHttpRequest.Setup(m => m.RouteValues).Returns(new RouteValueDictionary());
            mockHttpContext.Setup(m => m.Request).Returns(mockHttpRequest.Object);

            // Act / Assert
            Assert.Throws<ArgumentException>(()=> mockHttpContext.Object.GetRouteValueString(valueKey));

        }
    }
}
