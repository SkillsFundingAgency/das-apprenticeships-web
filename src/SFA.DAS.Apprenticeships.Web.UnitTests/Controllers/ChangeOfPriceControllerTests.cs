using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;
using SFA.DAS.Apprenticeships.Domain.Interfaces;
using SFA.DAS.Apprenticeships.Web.Controllers;
using SFA.DAS.Apprenticeships.Web.Infrastructure;
using SFA.DAS.Apprenticeships.Web.Models;
using SFA.DAS.Apprenticeships.Web.UnitTests.TestHelpers;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Controllers
{
    [TestFixture]
    public class ChangeOfPriceControllerTests
    {
        private readonly Fixture _fixture;
        private readonly ILogger<ChangeOfPriceController> _logger;
        private Mock<IApprenticeshipService> _mockApprenticeshipService = null!; // should be initialized in Setup()
        private Mock<IMapper<CreateChangeOfPriceModel>> _mockMapper = null!; // should be initialized in Setup()

        public ChangeOfPriceControllerTests()
        {
            _fixture = new Fixture();
            _logger = Mock.Of<ILogger<ChangeOfPriceController>>();
        }

        [SetUp]
        public void Setup()
        {
            _mockApprenticeshipService = new Mock<IApprenticeshipService>();
            _mockMapper = new Mock<IMapper<CreateChangeOfPriceModel>>();
        }

        [Test]
        public async Task GetProviderInitiatedPage_ReturnsMappedModel()
        {
            // Arrange
            var apprenticeshipPrice = _fixture.Create<ApprenticeshipPrice>();
            _mockApprenticeshipService.Setup(m => m.GetApprenticeshipPrice(It.IsAny<string>())).ReturnsAsync(apprenticeshipPrice);

            var createChangeOfPriceModel = _fixture.Create<CreateChangeOfPriceModel>();
            _mockMapper.Setup(m => m.Map(apprenticeshipPrice)).Returns(createChangeOfPriceModel);

            var controller = new ChangeOfPriceController(_logger, _mockApprenticeshipService.Object, _mockMapper.Object);
            AddProviderInitiatedRouteValues(controller, "anyProviderReference", "anyApprenticeshipId");

            // Act
            var result = await controller.GetProviderInitiatedPage("anyApprenticeshipId");

            // Assert
            var viewResult = result.ShouldBeOfType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeOfType<CreateChangeOfPriceModel>();
            viewModel.Should().Be(createChangeOfPriceModel);
            viewModel.ProviderReferenceNumber.Should().Be("anyProviderReference");
            viewModel.ApprenticeshipHashedId.Should().Be("anyApprenticeshipId");
        }

        [Test]
        public void ProviderInitiatedPriceChangeRequest_InvalidModel_ReturnsCreatePriceChangeRequestView()
        {
            // Arrange
            var createChangeOfPriceModel = _fixture.Create<CreateChangeOfPriceModel>();
            var controller = new ChangeOfPriceController(_logger, _mockApprenticeshipService.Object, _mockMapper.Object);
            controller.ModelState.AddModelError("anyKey", "anyErrorMessage");
            AddProviderInitiatedRouteValues(controller, "anyProviderReference", "anyApprenticeshipId");

            // Act
            var result = controller.ProviderInitiatedPriceChangeRequest(createChangeOfPriceModel);

            // Assert
            var viewResult = result.ShouldBeOfType<ViewResult>();
            viewResult.ViewName.Should().Be(ChangeOfPriceController.ProviderInitiatedViewName);
        }

        private void AddProviderInitiatedRouteValues(ChangeOfPriceController controller, string providerReferenceNumber, string apprenticeshipHashedId)
        {
            if(controller.HttpContext == null)
            {
                var httpContext = new Mock<HttpContext>();
                var httpRequest = new Mock<HttpRequest>();
                httpRequest.Setup(m => m.RouteValues).Returns(new RouteValueDictionary());
                httpContext.Setup(m => m.Request).Returns(httpRequest.Object);

                controller.ControllerContext.HttpContext = httpContext.Object;
            }

            controller.HttpContext!.Request.RouteValues.Add(RouteValues.Ukprn, providerReferenceNumber);
            controller.HttpContext.Request.RouteValues.Add(RouteValues.ApprenticeshipHashedId, apprenticeshipHashedId);
        }
    }
}
