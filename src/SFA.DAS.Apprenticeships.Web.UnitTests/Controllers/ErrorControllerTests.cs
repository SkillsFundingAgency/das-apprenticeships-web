using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SFA.DAS.Apprenticeships.Web.Controllers;
using SFA.DAS.Apprenticeships.Web.Models.Error;
using Moq;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;
using SFA.DAS.Apprenticeships.Domain.Api;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Controllers;

[TestFixture]
public class ErrorControllerTests
{
    [Test]
    [TestCase("test", "https://test-services.signin.education.gov.uk/approvals/select-organisation?action=request-service", true)]
    [TestCase("pp", "https://test-services.signin.education.gov.uk/approvals/select-organisation?action=request-service", true)]
    [TestCase("local", "https://test-services.signin.education.gov.uk/approvals/select-organisation?action=request-service", false)]
    [TestCase("prd", "https://services.signin.education.gov.uk/approvals/select-organisation?action=request-service", false)]
    public void WhenStatusCodeIs403Then403ViewIsReturned(string env, string helpLink, bool useDfESignIn)
    {
        // Arrange
        var sut = CreateControllerWithCustomConfig(env, useDfESignIn);

        // Act
        var result = (ViewResult)sut.Error(403);

        // Assert
        Assert.That(result, Is.Not.Null);
        var actualModel = result?.Model as Error403ViewModel;
        Assert.That(actualModel?.HelpPageLink, Is.EqualTo(helpLink));
        //Assert.That(actualModel?.UseDfESignIn, Is.EqualTo(useDfESignIn));
    }

    [Test]
    public void WhenStatusCodeIs404Then404ViewIsReturned()
    {
        // Arrange
        var sut = CreateController();

        // Act
        var result = (ViewResult)sut.Error(404);
            
        // Assert
        result.ViewName.Should().Be("404");
    }

    [Test]
    public void WhenStatusCodeIs401Then401ViewIsReturned()
    {
        // Arrange
        var sut = CreateController();

        // Act
        var result = (ViewResult)sut.Error(401);

        // Assert
        result.ViewName.Should().Be("401");
    }

    [Test]
    public void WhenAuthErrorReturnedFromApiThen401ViewIsReturned()
    {
        // Arrange
        var httpContextMock = new Mock<HttpContext>();
        var featureCollection = new FeatureCollection();
        var exceptionHandlerFeatureMock = new Mock<IExceptionHandlerFeature>();

        var apiUnauthorizedException = new ApiUnauthorizedException();
        exceptionHandlerFeatureMock.SetupGet(x => x.Error).Returns(apiUnauthorizedException);

        featureCollection.Set(exceptionHandlerFeatureMock.Object);
        httpContextMock.SetupGet(x => x.Features).Returns(featureCollection);

        var sut = CreateController();
        sut.ControllerContext.HttpContext = httpContextMock.Object;

        // Act
        var result = (ViewResult)sut.Error(500);

        // Assert
        result.ViewName.Should().Be("401");
    }

    [TestCase(null)]
    [TestCase(503)]
    [TestCase(405)]
    public void WhenStatusCodeIsNotHandledThenGenericErrorViewIsReturned(int? errorCode)
    {
        // Arrange
        var sut = CreateController();
            
        // Act
        var result = (ViewResult)sut.Error(errorCode);
            
        // Assert
        result.ViewName.Should().BeNull();
    }

    private static ErrorController CreateController()
    {
        var mockConfiguration = new Mock<IConfiguration>();
        var controller = new ErrorController(mockConfiguration.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        return controller;
    }

    private static ErrorController CreateControllerWithCustomConfig(string env, bool useDfESignIn)
    {
        var mockConfiguration = new Mock<IConfiguration>();
        mockConfiguration.Setup(x => x["ResourceEnvironmentName"]).Returns(env);
        mockConfiguration.Setup(x => x["UseDfESignIn"]).Returns(Convert.ToString(useDfESignIn));
        return new ErrorController(mockConfiguration.Object);
    }
}