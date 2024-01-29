using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SFA.DAS.Apprenticeships.Web.Controllers;
using SFA.DAS.Apprenticeships.Web.Models.Error;
using Moq;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Controllers
{
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
            return new ErrorController(mockConfiguration.Object);
        }

        private static ErrorController CreateControllerWithCustomConfig(string env, bool useDfESignIn)
        {
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(x => x["ResourceEnvironmentName"]).Returns(env);
            mockConfiguration.Setup(x => x["UseDfESignIn"]).Returns(Convert.ToString(useDfESignIn));
            return new ErrorController(mockConfiguration.Object);
        }
    }
}
