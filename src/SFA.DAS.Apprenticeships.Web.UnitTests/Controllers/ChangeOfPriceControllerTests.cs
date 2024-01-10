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
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.Provider.Shared.UI.Extensions;
using SFA.DAS.Provider.Shared.UI.Models;

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
        private Mock<IExternalUrlHelper> _mockExternalUrlHelper = null!;
        private string _expectedProviderCommitmentsUrl = null!;

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

            _mockExternalUrlHelper = new Mock<IExternalUrlHelper>();
            _expectedProviderCommitmentsUrl = _fixture.Create<string>();
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

            _mockExternalUrlHelper.Setup(x => x.GenerateUrl(It.IsAny<UrlParameters>()))
                .Returns(_expectedProviderCommitmentsUrl);

            _mockCacheService = new Mock<ICacheService>();
            var controller = new ChangeOfPriceController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object, Mock.Of<UrlBuilder>());
            
            AddProviderInitiatedRouteValues(controller, _fixture.Create<long>(), apprenticeshipHashedId);

            // Act
            var result = await controller.GetProviderInitiatedPage(apprenticeshipHashedId);

            // Assert
            var viewResult = result.ShouldBeOfType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeOfType<CreateChangeOfPriceModel>();
            viewModel.Should().Be(createChangeOfPriceModel);
            viewModel.ApprenticeshipHashedId.Should().Be(apprenticeshipHashedId);
        }

        [Test]
        public async Task GetProviderInitiatedPage_HashIdNotFound_Returns404()
        {
            // Arrange
            var apprenticeshipHashedId = _fixture.Create<string>();
            var controller = new ChangeOfPriceController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object, Mock.Of<UrlBuilder>());

            // Act
            var result = await controller.GetProviderInitiatedPage(apprenticeshipHashedId);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
            _mockLogger.ShouldHaveLoggedMessage(LogLevel.Warning, $"Apprenticeship key not found for apprenticeship with hashed id {apprenticeshipHashedId}");
        }

        [Test]
        public async Task GetProviderInitiatedPage_ApprenticeshipPriceNotFound_Returns404()
        {
            // Arrange
            var apprenticeshipHashedId = _fixture.Create<string>();

            var apprenticeshipKey = _fixture.Create<Guid>();
            _mockApprenticeshipService.Setup(m => m.GetApprenticeshipKey(apprenticeshipHashedId)).ReturnsAsync(apprenticeshipKey);

            var controller = new ChangeOfPriceController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object, Mock.Of<UrlBuilder>());

            // Act
            var result = await controller.GetProviderInitiatedPage(apprenticeshipHashedId);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
            _mockLogger.ShouldHaveLoggedMessage(LogLevel.Warning, $"ApprenticeshipPrice not found for apprenticeshipKey {apprenticeshipKey}");
        }
        
        [Test]
        public async Task ProviderInitiatedCheckDetailsPage_InvalidModel_ReturnsProviderInitiatedViewName()
        {
            // Arrange
            var createChangeOfPriceModel = _fixture.Create<CreateChangeOfPriceModel>();
            var controller = new ChangeOfPriceController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object, Mock.Of<UrlBuilder>());
            controller.ModelState.AddModelError("anyKey", "anyErrorMessage");
            AddProviderInitiatedRouteValues(controller, _fixture.Create<long>(), "anyApprenticeshipId");

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
			var controller = new ChangeOfPriceController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object, Mock.Of<UrlBuilder>());
			AddProviderInitiatedRouteValues(controller, _fixture.Create<long>(), "anyApprenticeshipId");

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
			var controller = new ChangeOfPriceController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object, Mock.Of<UrlBuilder>());

			// Act
			var result = controller.GetProviderInitiatedEditPage(createChangeOfPriceModel);

			// Assert
			var viewResult = result.ShouldBeOfType<ViewResult>();
			viewResult.ViewName.Should().Be(ChangeOfPriceController.ProviderInitiatedViewName);
        }
        
        [Test]
        public async Task ProviderInitiatedSubmitChange_ValidModel_CreatesPriceHistoryAndRedirectsToProviderCommitments()
        {
            // Arrange
            var createChangeOfPriceModel = _fixture.Create<CreateChangeOfPriceModel>();
            var controller = new ChangeOfPriceController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object, Mock.Of<UrlBuilder>());
            AddProviderInitiatedRouteValues(controller, _fixture.Create<long>(), "anyApprenticeshipId");
            var expectedUrl = _fixture.Create<string>();
            _mockExternalUrlHelper.Setup(x => x.GenerateUrl(It.IsAny<UrlParameters>())).Returns(expectedUrl);

            var expectedUser = _fixture.Create<string>();
            controller.SetUserName(expectedUser);


            // Act
            var result = await controller.ProviderInitiatedSubmitChange(createChangeOfPriceModel);

            // Assert
            _mockApprenticeshipService.Verify(x => x.CreatePriceHistory(
                createChangeOfPriceModel.ApprenticeshipKey, 
                createChangeOfPriceModel.ProviderReferenceNumber, 
                null,
                expectedUser,
                createChangeOfPriceModel.ApprenticeshipTrainingPrice,
                createChangeOfPriceModel.ApprenticeshipEndPointAssessmentPrice,
                createChangeOfPriceModel.ApprenticeshipTotalPrice,
                It.IsAny<string>(),
                createChangeOfPriceModel.EffectiveFromDate.Date.GetValueOrDefault()));
            result.ShouldBeOfType<RedirectResult>();
            ((RedirectResult)result).Url.Should().Be(expectedUrl);
		}

        [Test]
        public async Task GetViewPendingPriceChangePageProvider_ReturnsCorrectView()
        {
            // Arrange
            var ukprn = _fixture.Create<long>();
            var apprenticeshipHashedId = _fixture.Create<string>();
            var apprenticeshipKey = Guid.NewGuid();
            var pendingPriceChange = _fixture.Create<GetPendingPriceChangeResponse>();

            _mockApprenticeshipService.Setup(x => x.GetApprenticeshipKey(apprenticeshipHashedId))
                .ReturnsAsync(apprenticeshipKey);

            _mockApprenticeshipService.Setup(x => x.GetPendingPriceChange(apprenticeshipKey))
                .ReturnsAsync(pendingPriceChange);

            var controller = new ChangeOfPriceController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object);

            // Act
            var result = await controller.GetViewPendingPriceChangePageProvider(ukprn, apprenticeshipHashedId);

            // Assert
            var viewResult = result.ShouldBeOfType<ViewResult>();
            viewResult.ViewName.Should().Be(ChangeOfPriceController.ProviderViewPendingViewName);
            viewResult.Model.ShouldBeOfType<ProviderViewPendingPriceChangeModel>();
        }

        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public async Task GetViewPendingPriceChangePageProvider_ReturnsNotFoundWhenMissingApprenticeshipOrPriceChange(bool returnValidKey, bool returnValidPriceChange)
        {
            // Arrange
            var ukprn = _fixture.Create<long>();
            var apprenticeshipHashedId = _fixture.Create<string>();
            var apprenticeshipKey = Guid.NewGuid();
            var pendingPriceChange = _fixture.Create<GetPendingPriceChangeResponse>();

            if(returnValidKey)
                _mockApprenticeshipService.Setup(x => x.GetApprenticeshipKey(apprenticeshipHashedId))
                    .ReturnsAsync(apprenticeshipKey);

            if(returnValidPriceChange)
                _mockApprenticeshipService.Setup(x => x.GetPendingPriceChange(apprenticeshipKey))
                    .ReturnsAsync(pendingPriceChange);

            var controller = new ChangeOfPriceController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object);

            // Act
            var result = await controller.GetViewPendingPriceChangePageProvider(ukprn, apprenticeshipHashedId);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Test]
        public async Task GetViewPendingPriceChangePageEmployer_ReturnsCorrectView()
        {
            // Arrange
            var accountId = _fixture.Create<long>();
            var apprenticeshipHashedId = _fixture.Create<string>();
            var apprenticeshipKey = Guid.NewGuid();
            var pendingPriceChange = _fixture.Create<GetPendingPriceChangeResponse>();

            _mockApprenticeshipService.Setup(x => x.GetApprenticeshipKey(apprenticeshipHashedId))
                .ReturnsAsync(apprenticeshipKey);

            _mockApprenticeshipService.Setup(x => x.GetPendingPriceChange(apprenticeshipKey))
                .ReturnsAsync(pendingPriceChange);

            var controller = new ChangeOfPriceController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object);

            // Act
            var result = await controller.GetViewPendingPriceChangePageEmployer(accountId, apprenticeshipHashedId);

            // Assert
            var viewResult = result.ShouldBeOfType<ViewResult>();
            viewResult.ViewName.Should().Be(ChangeOfPriceController.EmployerViewPendingViewName);
            viewResult.Model.ShouldBeOfType<EmployerViewPendingPriceChangeModel>();
        }

        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public async Task GetViewPendingPriceChangePageEmployer_ReturnsNotFoundWhenMissingApprenticeshipOrPriceChange(bool returnValidKey, bool returnValidPriceChange)
        {
            // Arrange
            var accountId = _fixture.Create<long>();
            var apprenticeshipHashedId = _fixture.Create<string>();
            var apprenticeshipKey = Guid.NewGuid();
            var pendingPriceChange = _fixture.Create<GetPendingPriceChangeResponse>();

            if (returnValidKey)
                _mockApprenticeshipService.Setup(x => x.GetApprenticeshipKey(apprenticeshipHashedId))
                    .ReturnsAsync(apprenticeshipKey);

            if (returnValidPriceChange)
                _mockApprenticeshipService.Setup(x => x.GetPendingPriceChange(apprenticeshipKey))
                    .ReturnsAsync(pendingPriceChange);

            var controller = new ChangeOfPriceController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object);

            // Act
            var result = await controller.GetViewPendingPriceChangePageEmployer(accountId, apprenticeshipHashedId);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        private void AddProviderInitiatedRouteValues(ChangeOfPriceController controller, long providerReferenceNumber, string apprenticeshipHashedId)
        {
            if(controller.HttpContext == null)
            {
                var httpContext = new Mock<HttpContext>();
                var httpRequest = new Mock<HttpRequest>();
                httpRequest.Setup(m => m.RouteValues).Returns(new RouteValueDictionary());
                httpContext.Setup(m => m.Request).Returns(httpRequest.Object);

                controller.ControllerContext.HttpContext = httpContext.Object;
            }

            controller.HttpContext!.Request.RouteValues.Add(RouteValues.Ukprn, providerReferenceNumber.ToString());
            controller.HttpContext.Request.RouteValues.Add(RouteValues.ApprenticeshipHashedId, apprenticeshipHashedId);
        }
    }
}
