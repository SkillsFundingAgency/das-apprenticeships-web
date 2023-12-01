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
using SFA.DAS.Apprenticeships.Web.Services;
using SFA.DAS.Apprenticeships.Web.UnitTests.TestHelpers;
using System.Runtime.CompilerServices;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Controllers
{
    [TestFixture]
    public class ChangeOfPriceControllerTests
    {
        private readonly Fixture _fixture;
        private readonly Mock<ILogger<ChangeOfPriceController>> _mockLogger;
        private Mock<IApprenticeshipService> _mockApprenticeshipService = null!; // should be initialized in Setup()
        private Mock<IMapper<CreateChangeOfPriceModel>> _mockMapper = null!; // should be initialized in Setup()
		private Mock<ICacheService> _mockCacheService = null!; // should be initialized in Setup()

		public ChangeOfPriceControllerTests()
        {
            _fixture = new Fixture();
            _mockLogger = new Mock<ILogger<ChangeOfPriceController>>();
        }

        [SetUp]
        public void Setup()
        {
            _mockApprenticeshipService = new Mock<IApprenticeshipService>();
            _mockMapper = new Mock<IMapper<CreateChangeOfPriceModel>>();
			_mockCacheService = new Mock<ICacheService>();

		}

        [Test]
        public async Task GetProviderInitiatedPage_ReturnsMappedModel()
        {
            // Arrange
            var apprenticeshipHashedId = _fixture.Create<string>();

            var apprenticeshipKey = _fixture.Create<Guid>();
            _mockApprenticeshipService.Setup(m => m.GetApprenticeshipKey(apprenticeshipHashedId)).ReturnsAsync(apprenticeshipKey);

            var apprenticeshipPrice = _fixture.Create<ApprenticeshipPrice>();
            apprenticeshipPrice.ApprenticeshipKey = apprenticeshipKey;
            _mockApprenticeshipService.Setup(m => m.GetApprenticeshipPrice(apprenticeshipKey)).ReturnsAsync(apprenticeshipPrice);

            var createChangeOfPriceModel = _fixture.Create<CreateChangeOfPriceModel>();
            _mockMapper.Setup(m => m.Map(apprenticeshipPrice)).Returns(createChangeOfPriceModel);

            var controller = new ChangeOfPriceController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object);
            AddProviderInitiatedRouteValues(controller, "anyProviderReference", apprenticeshipHashedId);

            // Act
            var result = await controller.GetProviderInitiatedPage(apprenticeshipHashedId);

            // Assert
            var viewResult = result.ShouldBeOfType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeOfType<CreateChangeOfPriceModel>();
            viewModel.Should().Be(createChangeOfPriceModel);
            viewModel.ProviderReferenceNumber.Should().Be("anyProviderReference");
            viewModel.ApprenticeshipHashedId.Should().Be(apprenticeshipHashedId);
        }

        [Test]
        public async Task GetProviderInitiatedPage_HashIdNotFound_Returns404()
        {
            // Arrange
            var apprenticeshipHashedId = _fixture.Create<string>();
            var controller = new ChangeOfPriceController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object);

            // Act
            var result = await controller.GetProviderInitiatedPage(apprenticeshipHashedId);

            // Assert
            var viewResult = result.ShouldBeOfType<NotFoundResult>();
            _mockLogger.ShouldHaveLoggedMessage(LogLevel.Warning, $"Apprenticeship key not found for apprenticeship with hashed id {apprenticeshipHashedId}");
        }

        [Test]
        public async Task GetProviderInitiatedPage_ApprenticeshipPriceNotFound_Returns404()
        {
            // Arrange
            var apprenticeshipHashedId = _fixture.Create<string>();

            var apprenticeshipKey = _fixture.Create<Guid>();
            _mockApprenticeshipService.Setup(m => m.GetApprenticeshipKey(apprenticeshipHashedId)).ReturnsAsync(apprenticeshipKey);

            var controller = new ChangeOfPriceController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object);

            // Act
            var result = await controller.GetProviderInitiatedPage(apprenticeshipHashedId);

            // Assert
            var viewResult = result.ShouldBeOfType<NotFoundResult>();
            _mockLogger.ShouldHaveLoggedMessage(LogLevel.Warning, $"ApprenticeshipPrice not found for apprenticeshipKey {apprenticeshipKey}");
        }
        
        [Test]
        public async Task ProviderInitiatedCheckDetailsPage_InvalidModel_ReturnsProviderInitiatedViewName()
        {
            // Arrange
            var createChangeOfPriceModel = _fixture.Create<CreateChangeOfPriceModel>();
            var controller = new ChangeOfPriceController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object);
            controller.ModelState.AddModelError("anyKey", "anyErrorMessage");
            AddProviderInitiatedRouteValues(controller, "anyProviderReference", "anyApprenticeshipId");

            // Act
            var result = await controller.ProviderInitiatedCheckDetailsPage(createChangeOfPriceModel);

            // Assert
            var viewResult = result.ShouldBeOfType<ViewResult>();
            viewResult.ViewName.Should().Be(ChangeOfPriceController.ProviderInitiatedViewName);
        }

		[Test]
		public async Task ProviderInitiatedCheckDetailsPage_ReturnsProviderInitiatedCheckDetailsViewName()
		{
			// Arrange
			var createChangeOfPriceModel = _fixture.Create<CreateChangeOfPriceModel>();
			var controller = new ChangeOfPriceController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object);
			AddProviderInitiatedRouteValues(controller, "anyProviderReference", "anyApprenticeshipId");

			// Act
			var result = await controller.ProviderInitiatedCheckDetailsPage(createChangeOfPriceModel);

			// Assert
			var viewResult = result.ShouldBeOfType<ViewResult>();
			viewResult.ViewName.Should().Be(ChangeOfPriceController.ProviderInitiatedCheckDetailsViewName);
		}

		[Test]
		public void GetProviderInitiatedEditPage_ReturnsProviderInitiatedViewName()
		{
			// Arrange
			var createChangeOfPriceModel = _fixture.Create<CreateChangeOfPriceModel>();
			var controller = new ChangeOfPriceController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object);

			// Act
			var result = controller.GetProviderInitiatedEditPage(createChangeOfPriceModel);

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
