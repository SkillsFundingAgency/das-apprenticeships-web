using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;
using SFA.DAS.Apprenticeships.Domain.Interfaces;
using SFA.DAS.Apprenticeships.Web.Controllers;
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
        private Mock<IMapper> _mockMapper = null!; // should be initialized in Setup()
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
            _mockMapper = new Mock<IMapper>();
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

            var createChangeOfPriceModel = _fixture.Create<ProviderChangeOfPriceModel>();
            _mockMapper.Setup(m => m.Map<ProviderChangeOfPriceModel>(apprenticeshipPrice)).Returns(createChangeOfPriceModel);

            _mockExternalUrlHelper.Setup(x => x.GenerateUrl(It.IsAny<UrlParameters>()))
                .Returns(_expectedProviderCommitmentsUrl);

            _mockCacheService = new Mock<ICacheService>();
            var controller = new ChangeOfPriceController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object, GetMockUrlBuilder());
            
            controller.SetupHttpContext(_fixture.Create<long>(), apprenticeshipHashedId);

            // Act
            var result = await controller.GetProviderInitiatedPage(apprenticeshipHashedId);

            // Assert
            var viewResult = result.ShouldBeOfType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeOfType<ProviderChangeOfPriceModel>();
            viewModel.Should().Be(createChangeOfPriceModel);
            viewModel.ApprenticeshipHashedId.Should().Be(apprenticeshipHashedId);
        }

        [Test]
        public async Task GetProviderInitiatedPage_HashIdNotFound_Returns404()
        {
            // Arrange
            var apprenticeshipHashedId = _fixture.Create<string>();
            var controller = new ChangeOfPriceController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object, GetMockUrlBuilder());

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

            var controller = new ChangeOfPriceController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object, GetMockUrlBuilder());

            // Act
            var result = await controller.GetProviderInitiatedPage(apprenticeshipHashedId);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
            _mockLogger.ShouldHaveLoggedMessage(LogLevel.Warning, $"ApprenticeshipPrice not found for apprenticeshipKey {apprenticeshipKey}");
        }

        [Test]
        public async Task GetEmployerInitiatedPage_ReturnsMappedModel()
        {
            // Arrange
            var employerAccountId = _fixture.Create<string>();
            var apprenticeshipHashedId = _fixture.Create<string>();

            var apprenticeshipKey = _fixture.Create<Guid>();
            _mockApprenticeshipService.Setup(m => m.GetApprenticeshipKey(apprenticeshipHashedId)).ReturnsAsync(apprenticeshipKey);

            var apprenticeshipPrice = _fixture.Create<ApprenticeshipPrice>();
            apprenticeshipPrice.ApprenticeshipKey = apprenticeshipKey;
            _mockApprenticeshipService.Setup(m => m.GetApprenticeshipPrice(apprenticeshipKey)).ReturnsAsync(apprenticeshipPrice);

            var createChangeOfPriceModel = _fixture.Create<EmployerChangeOfPriceModel>();
            _mockMapper.Setup(m => m.Map<EmployerChangeOfPriceModel>(apprenticeshipPrice)).Returns(createChangeOfPriceModel);

            _mockExternalUrlHelper.Setup(x => x.GenerateUrl(It.IsAny<UrlParameters>()))
                .Returns(_expectedProviderCommitmentsUrl);

            _mockCacheService = new Mock<ICacheService>();
            var controller = new ChangeOfPriceController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object, GetMockUrlBuilder());

            controller.SetupHttpContext(null, apprenticeshipHashedId,null, employerAccountId);

            // Act
            var result = await controller.GetEmployerInitiatedPage(apprenticeshipHashedId);

            // Assert
            var viewResult = result.ShouldBeOfType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeOfType<EmployerChangeOfPriceModel>();
            viewModel.Should().Be(createChangeOfPriceModel);
            viewModel.ApprenticeshipHashedId.Should().Be(apprenticeshipHashedId);
        }

        [Test]
        public async Task GetEmployerInitiatedPage_HashIdNotFound_Returns404()
        {
            // Arrange
            var apprenticeshipHashedId = _fixture.Create<string>();
            var controller = new ChangeOfPriceController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object, GetMockUrlBuilder());

            // Act
            var result = await controller.GetEmployerInitiatedPage(apprenticeshipHashedId);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
            _mockLogger.ShouldHaveLoggedMessage(LogLevel.Warning, $"Apprenticeship key not found for apprenticeship with hashed id {apprenticeshipHashedId}");
        }

        [Test]
        public async Task GetEmployerInitiatedPage_ApprenticeshipPriceNotFound_Returns404()
        {
            // Arrange
            var apprenticeshipHashedId = _fixture.Create<string>();

            var apprenticeshipKey = _fixture.Create<Guid>();
            _mockApprenticeshipService.Setup(m => m.GetApprenticeshipKey(apprenticeshipHashedId)).ReturnsAsync(apprenticeshipKey);

            var controller = new ChangeOfPriceController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object, GetMockUrlBuilder());

            // Act
            var result = await controller.GetEmployerInitiatedPage(apprenticeshipHashedId);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
            _mockLogger.ShouldHaveLoggedMessage(LogLevel.Warning, $"ApprenticeshipPrice not found for apprenticeshipKey {apprenticeshipKey}");
        }

        [Test]
        public async Task ProviderInitiatedCheckDetailsPage_InvalidModel_ReturnsProviderInitiatedViewName()
        {
            // Arrange
            var createChangeOfPriceModel = _fixture.Create<ProviderChangeOfPriceModel>();
            var controller = new ChangeOfPriceController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object, GetMockUrlBuilder());
            controller.ModelState.AddModelError("anyKey", "anyErrorMessage");
            controller.SetupHttpContext(_fixture.Create<long>(), "anyApprenticeshipId");

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
			var createChangeOfPriceModel = _fixture.Create<ProviderChangeOfPriceModel>();
			var controller = new ChangeOfPriceController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object, GetMockUrlBuilder());
            controller.SetupHttpContext(_fixture.Create<long>(), "anyApprenticeshipId");

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
			var createChangeOfPriceModel = _fixture.Create<ProviderChangeOfPriceModel>();
			var controller = new ChangeOfPriceController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object, GetMockUrlBuilder());

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
            var expectedUser = _fixture.Create<string>();

            var createChangeOfPriceModel = _fixture.Create<ProviderChangeOfPriceModel>();
            var controller = new ChangeOfPriceController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object, GetMockUrlBuilder());
            controller.SetupHttpContext(_fixture.Create<long>(), "anyApprenticeshipId", expectedUser);
            var expectedUrl = _fixture.Create<string>();
            _mockExternalUrlHelper.Setup(x => x.GenerateUrl(It.IsAny<UrlParameters>())).Returns(expectedUrl);


            // Act
            var result = await controller.ProviderInitiatedSubmitChange(createChangeOfPriceModel);

            // Assert
            _mockApprenticeshipService.Verify(x => x.CreatePriceHistory(
                createChangeOfPriceModel.ApprenticeshipKey, 
                "Provider",
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
        public async Task EmployerInitiatedSubmitChange_ValidModel_CreatesPriceHistoryAndRedirectsToEmployerCommitments()
        {
            // Arrange
            var expectedUser = _fixture.Create<string>();

            var createChangeOfPriceModel = _fixture.Create<EmployerChangeOfPriceModel>();
            var controller = new ChangeOfPriceController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object, GetMockUrlBuilder());
            controller.SetupHttpContext(_fixture.Create<long>(), "anyApprenticeshipId", expectedUser);


            // Act
            var result = await controller.EmployerInitiatedSubmitChange(createChangeOfPriceModel);

            // Assert
            _mockApprenticeshipService.Verify(x => x.CreatePriceHistory(
                createChangeOfPriceModel.ApprenticeshipKey,
                "Employer",
                expectedUser,
                null,
                null,
                createChangeOfPriceModel.ApprenticeshipTotalPrice,
                It.IsAny<string>(),
                createChangeOfPriceModel.EffectiveFromDate.Date.GetValueOrDefault()));
            result.ShouldBeOfType<RedirectResult>();
            ((RedirectResult)result).Url.Should().EndWith("?showChangeOfPriceRequestSent=true");
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

            var controller = new ChangeOfPriceController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object, GetMockUrlBuilder());

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

            var controller = new ChangeOfPriceController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object, GetMockUrlBuilder());

            // Act
            var result = await controller.GetViewPendingPriceChangePageProvider(ukprn, apprenticeshipHashedId);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Test]
        public async Task GetViewPendingPriceChangePageEmployer_ReturnsCorrectView()
        {
            // Arrange
            var accountId = _fixture.Create<string>();
            var apprenticeshipHashedId = _fixture.Create<string>();
            var apprenticeshipKey = Guid.NewGuid();
            var pendingPriceChange = _fixture.Create<GetPendingPriceChangeResponse>();

            _mockApprenticeshipService.Setup(x => x.GetApprenticeshipKey(apprenticeshipHashedId))
                .ReturnsAsync(apprenticeshipKey);

            _mockApprenticeshipService.Setup(x => x.GetPendingPriceChange(apprenticeshipKey))
                .ReturnsAsync(pendingPriceChange);

            var controller = new ChangeOfPriceController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object, GetMockUrlBuilder());

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
            var accountId = _fixture.Create<string>();
            var apprenticeshipHashedId = _fixture.Create<string>();
            var apprenticeshipKey = Guid.NewGuid();
            var pendingPriceChange = _fixture.Create<GetPendingPriceChangeResponse>();

            if (returnValidKey)
                _mockApprenticeshipService.Setup(x => x.GetApprenticeshipKey(apprenticeshipHashedId))
                    .ReturnsAsync(apprenticeshipKey);

            if (returnValidPriceChange)
                _mockApprenticeshipService.Setup(x => x.GetPendingPriceChange(apprenticeshipKey))
                    .ReturnsAsync(pendingPriceChange);

            var controller = new ChangeOfPriceController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object, GetMockUrlBuilder());

            // Act
            var result = await controller.GetViewPendingPriceChangePageEmployer(accountId, apprenticeshipHashedId);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Test]
        public async Task ProviderCancelChange_CancelTrue_CancelsPriceHistoryAndRedirectsToProviderCommitments()
        {
            // Arrange
            var providerReferenceNumber = _fixture.Create<long>();
            var apprenticeshipHashedId = _fixture.Create<string>();
            var controller = new ChangeOfPriceController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object, GetMockUrlBuilder());
            controller.SetupHttpContext(providerReferenceNumber, apprenticeshipHashedId);
            var expectedUrl = _fixture.Create<string>();
            _mockExternalUrlHelper.Setup(x => x.GenerateUrl(It.IsAny<UrlParameters>())).Returns(expectedUrl);
            var apprenticeshipKey = _fixture.Create<Guid>();
            _mockApprenticeshipService.Setup(x => x.GetApprenticeshipKey(It.IsAny<string>())).ReturnsAsync(apprenticeshipKey);

            // Act
            var result = await controller.PostViewPendingPriceChangePage(providerReferenceNumber, apprenticeshipHashedId, "1");

            // Assert
            _mockApprenticeshipService.Verify(x => x.CancelPendingPriceChange(apprenticeshipKey), Times.Once);
            result.ShouldBeOfType<RedirectResult>();
            ((RedirectResult)result).Url.Should().Be(expectedUrl);
        }

        [Test]
        public async Task ProviderCancelChange_CancelFalse_DoesNotCancelPriceHistoryAndRedirectsToProviderCommitments()
        {
            // Arrange
            var providerReferenceNumber = _fixture.Create<long>();
            var apprenticeshipHashedId = _fixture.Create<string>();
            var providerUserName = _fixture.Create<string>();

            var controller = new ChangeOfPriceController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object, GetMockUrlBuilder());
            controller.SetupHttpContext(providerReferenceNumber, apprenticeshipHashedId, providerUserName);
            var expectedUrl = _fixture.Create<string>();
            _mockExternalUrlHelper.Setup(x => x.GenerateUrl(It.IsAny<UrlParameters>())).Returns(expectedUrl);
            var apprenticeshipKey = _fixture.Create<Guid>();
            _mockApprenticeshipService.Setup(x => x.GetApprenticeshipKey(It.IsAny<string>())).ReturnsAsync(apprenticeshipKey);

            // Act
            var result = await controller.PostViewPendingPriceChangePage(providerReferenceNumber, apprenticeshipHashedId, "0");

            // Assert
            _mockApprenticeshipService.Verify(x => x.CancelPendingPriceChange(apprenticeshipKey), Times.Never);
            result.ShouldBeOfType<RedirectResult>();
            ((RedirectResult)result).Url.Should().Be(expectedUrl);
        }

        [Test]
        public async Task EmployerRejectChange_ApproveFalse_RejectsPriceHistoryAndRedirectsToEmployerCommitments()
        {
            // Arrange
            var employerAccountId = _fixture.Create<string>();
            var apprenticeshipHashedId = _fixture.Create<string>();
            var controller = new ChangeOfPriceController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object, GetMockUrlBuilder());
            var apprenticeshipKey = _fixture.Create<Guid>();
            _mockApprenticeshipService.Setup(x => x.GetApprenticeshipKey(It.IsAny<string>())).ReturnsAsync(apprenticeshipKey);
            var rejectReason = _fixture.Create<string>();

            // Act
            var result = await controller.PostViewPendingPriceChangePageEmployer(employerAccountId, apprenticeshipHashedId, "0", rejectReason);

            // Assert
            _mockApprenticeshipService.Verify(x => x.RejectPendingPriceChange(apprenticeshipKey, rejectReason), Times.Once);
            result.ShouldBeOfType<RedirectResult>();
            var redirectResult = ((RedirectResult)result);
            redirectResult.Url.Should().Be($"https://approvals.at-eas.apprenticeships.education.gov.uk/{employerAccountId}/apprentices/{apprenticeshipHashedId}/details?showPriceChangeRejected=true");
        }

        private UrlBuilder GetMockUrlBuilder()
        {
            return new UrlBuilder("AT");
        }
    }
}
