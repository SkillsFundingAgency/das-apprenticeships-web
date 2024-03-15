using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;
using SFA.DAS.Apprenticeships.Domain.Interfaces;
using SFA.DAS.Apprenticeships.Web.Controllers;
using SFA.DAS.Apprenticeships.Web.Models;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfPrice;
using SFA.DAS.Apprenticeships.Web.Services;
using SFA.DAS.Apprenticeships.Web.UnitTests.TestHelpers;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.Provider.Shared.UI.Extensions;
using SFA.DAS.Provider.Shared.UI.Models;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Controllers
{
    [TestFixture]
    public class ChangeOfPriceProviderControllerTests
    {
        private readonly Fixture _fixture;
        private readonly Mock<ILogger<ChangeOfPriceProviderController>> _mockLogger;
        private Mock<IApprenticeshipService> _mockApprenticeshipService = null!; // should be initialized in Setup()
        private Mock<IMapper> _mockMapper = null!; // should be initialized in Setup()
        private Mock<ICacheService> _mockCacheService = null!; // should be initialized in Setup()
        private Mock<IExternalUrlHelper> _mockExternalUrlHelper = null!;
        private string _expectedProviderCommitmentsUrl = null!;

        public ChangeOfPriceProviderControllerTests()
        {
            _fixture = new Fixture();
            _mockLogger = new Mock<ILogger<ChangeOfPriceProviderController>>();
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
            var controller = new ChangeOfPriceProviderController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object);

            controller.SetupHttpContext(_fixture.Create<long>(), apprenticeshipHashedId);

            // Act
            var result = await controller.GetProviderEnterChangeDetails(apprenticeshipHashedId);

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
            var controller = new ChangeOfPriceProviderController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object);

            // Act
            var result = await controller.GetProviderEnterChangeDetails(apprenticeshipHashedId);

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

            var controller = new ChangeOfPriceProviderController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object);

            // Act
            var result = await controller.GetProviderEnterChangeDetails(apprenticeshipHashedId);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
            _mockLogger.ShouldHaveLoggedMessage(LogLevel.Warning, $"ApprenticeshipPrice not found for apprenticeshipKey {apprenticeshipKey}");
        }

        [Test]
        public async Task ProviderCheckDetails_InvalidModel_ReturnsProviderInitiatedViewName()
        {
            // Arrange
            var createChangeOfPriceModel = _fixture.Create<ProviderChangeOfPriceModel>();
            var controller = new ChangeOfPriceProviderController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object);
            controller.ModelState.AddModelError("anyKey", "anyErrorMessage");
            controller.SetupHttpContext(_fixture.Create<long>(), "anyApprenticeshipId");

            // Act
            var result = await controller.ProviderCheckDetails(createChangeOfPriceModel);

            // Assert
            var viewResult = result.ShouldBeOfType<ViewResult>();
            viewResult.ViewName.Should().Be(ChangeOfPriceProviderController.ProviderEnterChangeDetailsViewName);
        }

        [Test]
        public async Task ProviderCheckDetails_ReturnsProviderInitiatedCheckDetailsViewName()
        {
            // Arrange
            var createChangeOfPriceModel = _fixture.Create<ProviderChangeOfPriceModel>();
            var controller = new ChangeOfPriceProviderController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object);
            controller.SetupHttpContext(_fixture.Create<long>(), "anyApprenticeshipId");

            // Act
            var result = await controller.ProviderCheckDetails(createChangeOfPriceModel);

            // Assert
            var viewResult = result.ShouldBeOfType<ViewResult>();
            viewResult.ViewName.Should().Be(ChangeOfPriceProviderController.ProviderCheckDetailsViewName);
        }

        [Test]
        public void GetProviderInitiatedEditPage_ReturnsProviderInitiatedViewName()
        {
            // Arrange
            var createChangeOfPriceModel = _fixture.Create<ProviderChangeOfPriceModel>();
            var controller = new ChangeOfPriceProviderController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object);

            // Act
            var result = controller.GetProviderEditChangeDetails(createChangeOfPriceModel);

            // Assert
            var viewResult = result.ShouldBeOfType<ViewResult>();
            viewResult.ViewName.Should().Be(ChangeOfPriceProviderController.ProviderEnterChangeDetailsViewName);
        }

        [Test]
        public async Task ProviderInitiatedSubmitChange_ValidModel_CreatesPriceHistoryAndRedirectsToProviderCommitments()
        {
            // Arrange
            var expectedUser = _fixture.Create<string>();

            var createChangeOfPriceModel = _fixture.Create<ProviderChangeOfPriceModel>();
            var controller = new ChangeOfPriceProviderController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object);
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
        public async Task ViewPendingPriceChangePage_ReturnsCorrectView()
        {
            // Arrange
            var ukprn = _fixture.Create<long>();
            var apprenticeshipHashedId = _fixture.Create<string>();
            var apprenticeshipKey = Guid.NewGuid();
            var pendingPriceChange = _fixture.Create<GetPendingPriceChangeResponse>();
            pendingPriceChange.PendingPriceChange.Initiator = "Provider";

            _mockApprenticeshipService.Setup(x => x.GetApprenticeshipKey(apprenticeshipHashedId))
                .ReturnsAsync(apprenticeshipKey);

            _mockApprenticeshipService.Setup(x => x.GetPendingPriceChange(apprenticeshipKey))
                .ReturnsAsync(pendingPriceChange);

            _mockMapper.Setup(x => x.Map<ProviderCancelPriceChangeModel>(pendingPriceChange))
                .Returns(new ProviderCancelPriceChangeModel());

            var controller = new ChangeOfPriceProviderController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object);
            controller.SetupHttpContext(_fixture.Create<long>(), apprenticeshipHashedId);

            // Act
            var result = await controller.ViewPendingPriceChangePage(ukprn, apprenticeshipHashedId);

            // Assert
            var viewResult = result.ShouldBeOfType<ViewResult>();
            viewResult.ViewName.Should().Be(ChangeOfPriceProviderController.ProviderCancelPendingChangeViewName);
            viewResult.Model.ShouldBeOfType<ProviderCancelPriceChangeModel>();
        }

        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public async Task ViewPendingPriceChangePage_ReturnsNotFoundWhenMissingApprenticeshipOrPriceChange(bool returnValidKey, bool returnValidPriceChange)
        {
            // Arrange
            var ukprn = _fixture.Create<long>();
            var apprenticeshipHashedId = _fixture.Create<string>();
            var apprenticeshipKey = Guid.NewGuid();
            var pendingPriceChange = _fixture.Create<GetPendingPriceChangeResponse>();

            if (returnValidKey)
                _mockApprenticeshipService.Setup(x => x.GetApprenticeshipKey(apprenticeshipHashedId))
                    .ReturnsAsync(apprenticeshipKey);

            if (returnValidPriceChange)
                _mockApprenticeshipService.Setup(x => x.GetPendingPriceChange(apprenticeshipKey))
                    .ReturnsAsync(pendingPriceChange);

            var controller = new ChangeOfPriceProviderController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object);

            // Act
            var result = await controller.ViewPendingPriceChangePage(ukprn, apprenticeshipHashedId);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Test]
        public async Task ProviderCancelChange_CancelTrue_CancelsPriceHistoryAndRedirectsToProviderCommitments()
        {
            // Arrange
            var providerReferenceNumber = _fixture.Create<long>();
            var apprenticeshipHashedId = _fixture.Create<string>();
            var controller = new ChangeOfPriceProviderController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object);
            controller.SetupHttpContext(providerReferenceNumber, apprenticeshipHashedId);
            var expectedUrl = _fixture.Create<string>();
            _mockExternalUrlHelper.Setup(x => x.GenerateUrl(It.IsAny<UrlParameters>())).Returns(expectedUrl);
            var apprenticeshipKey = _fixture.Create<Guid>();
            _mockApprenticeshipService.Setup(x => x.GetApprenticeshipKey(It.IsAny<string>())).ReturnsAsync(apprenticeshipKey);

            // Act
            var result = await controller.CancelPriceChange(providerReferenceNumber, apprenticeshipHashedId, "1");

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

            var controller = new ChangeOfPriceProviderController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object);
            controller.SetupHttpContext(providerReferenceNumber, apprenticeshipHashedId, providerUserName);
            var expectedUrl = _fixture.Create<string>();
            _mockExternalUrlHelper.Setup(x => x.GenerateUrl(It.IsAny<UrlParameters>())).Returns(expectedUrl);
            var apprenticeshipKey = _fixture.Create<Guid>();
            _mockApprenticeshipService.Setup(x => x.GetApprenticeshipKey(It.IsAny<string>())).ReturnsAsync(apprenticeshipKey);

            // Act
            var result = await controller.CancelPriceChange(providerReferenceNumber, apprenticeshipHashedId, "0");

            // Assert
            _mockApprenticeshipService.Verify(x => x.CancelPendingPriceChange(apprenticeshipKey), Times.Never);
            result.ShouldBeOfType<RedirectResult>();
            ((RedirectResult)result).Url.Should().Be(expectedUrl);
        }
    }
}