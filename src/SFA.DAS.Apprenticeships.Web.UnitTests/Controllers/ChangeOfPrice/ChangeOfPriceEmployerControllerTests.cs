using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Responses;
using SFA.DAS.Apprenticeships.Domain.Interfaces;
using SFA.DAS.Apprenticeships.Web.Controllers.ChangeOfPrice;
using SFA.DAS.Apprenticeships.Web.Models;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfPrice;
using SFA.DAS.Apprenticeships.Web.Services;
using SFA.DAS.Apprenticeships.Web.UnitTests.TestHelpers;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.Provider.Shared.UI.Extensions;
using SFA.DAS.Provider.Shared.UI.Models;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Controllers.ChangeOfPrice
{
    [TestFixture]
    public class ChangeOfPriceEmployerControllerTests
    {
        private readonly Fixture _fixture;
        private readonly Mock<ILogger<ChangeOfPriceEmployerController>> _mockLogger;
        private Mock<IApprenticeshipService> _mockApprenticeshipService = null!; // should be initialized in Setup()
        private Mock<IMapper> _mockMapper = null!; // should be initialized in Setup()
        private Mock<ICacheService> _mockCacheService = null!; // should be initialized in Setup()
        private Mock<IExternalUrlHelper> _mockExternalUrlHelper = null!;
        private string _expectedProviderCommitmentsUrl = null!;

        public ChangeOfPriceEmployerControllerTests()
        {
            _fixture = new Fixture();
            _mockLogger = new Mock<ILogger<ChangeOfPriceEmployerController>>();
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
            var controller = new ChangeOfPriceEmployerController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, GetUrlBuilder());

            controller.SetupHttpContext(null, apprenticeshipHashedId, null, employerAccountId);

            // Act
            var result = await controller.GetEmployerEnterChangeDetails(apprenticeshipHashedId);

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
            var controller = new ChangeOfPriceEmployerController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, GetUrlBuilder());

            // Act
            var result = await controller.GetEmployerEnterChangeDetails(apprenticeshipHashedId);

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

            var controller = new ChangeOfPriceEmployerController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, GetUrlBuilder());

            // Act
            var result = await controller.GetEmployerEnterChangeDetails(apprenticeshipHashedId);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
            _mockLogger.ShouldHaveLoggedMessage(LogLevel.Warning, $"ApprenticeshipPrice not found for apprenticeshipKey {apprenticeshipKey}");
        }

        [Test]
        public async Task EmployerInitiatedSubmitChange_ValidModel_CreatesPriceHistoryAndRedirectsToEmployerCommitments()
        {
            // Arrange
            var expectedUser = _fixture.Create<string>();

            var createChangeOfPriceModel = _fixture.Create<EmployerChangeOfPriceModel>();
            var controller = new ChangeOfPriceEmployerController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, GetUrlBuilder());
            controller.SetupHttpContext(_fixture.Create<long>(), "anyApprenticeshipId", expectedUser);


            // Act
            var result = await controller.EmployerInitiatedSubmitChange(createChangeOfPriceModel);

            // Assert
            _mockApprenticeshipService.Verify(x => x.CreatePriceHistory(
                createChangeOfPriceModel.ApprenticeshipKey,
                "Employer",
                It.IsAny<string>(),
                null,
                null,
                createChangeOfPriceModel.ApprenticeshipTotalPrice,
                It.IsAny<string>(),
                createChangeOfPriceModel.EffectiveFromDate.Date.GetValueOrDefault()));
            result.ShouldBeOfType<RedirectResult>();
            ((RedirectResult)result).Url.Should().EndWith("?showChangeOfPriceRequestSent=true");
        }

        [Test]
        public async Task GetViewPendingPriceChangePageEmployer_ReturnsCorrectView()
        {
            // Arrange
            var accountId = _fixture.Create<string>();
            var apprenticeshipHashedId = _fixture.Create<string>();
            var employerAccountId = _fixture.Create<string>();
            var apprenticeshipKey = Guid.NewGuid();
            var pendingPriceChange = _fixture.Create<GetPendingPriceChangeResponse>();
            var viewModel = _fixture.Create<EmployerCancelPriceChangeModel>();
            pendingPriceChange.PendingPriceChange.Initiator = "Employer";

            _mockApprenticeshipService.Setup(x => x.GetApprenticeshipKey(apprenticeshipHashedId))
                .ReturnsAsync(apprenticeshipKey);

            _mockApprenticeshipService.Setup(x => x.GetPendingPriceChange(apprenticeshipKey))
                .ReturnsAsync(pendingPriceChange);

            _mockMapper.Setup(x => x.Map<EmployerCancelPriceChangeModel>(pendingPriceChange))
                .Returns(viewModel);

            var controller = new ChangeOfPriceEmployerController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, GetUrlBuilder());
            controller.SetupHttpContext(null, apprenticeshipHashedId, null, employerAccountId);

            // Act
            var result = await controller.ViewPendingPriceChangePage(accountId, apprenticeshipHashedId);

            // Assert
            var viewResult = result.ShouldBeOfType<ViewResult>();
            viewResult.ViewName.Should().Be(ChangeOfPriceEmployerController.CancelPendingChangeViewName);
            viewResult.Model.ShouldBeOfType<EmployerCancelPriceChangeModel>();
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

            var controller = new ChangeOfPriceEmployerController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, GetUrlBuilder());

            // Act
            var result = await controller.ViewPendingPriceChangePage(accountId, apprenticeshipHashedId);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [TestCase("<h3>test</h3>", "&lt;h3&gt;test&lt;/h3&gt;")]
        [TestCase("test", "test")]
        [TestCase(" ", " ")]
        [TestCase(null, "")]
        public async Task EmployerRejectChange_ApproveFalse_RejectsPriceHistoryAndRedirectsToEmployerCommitments(string? rejectReason, string? expectedEncodedReason)
        {
            // Arrange
            var employerAccountId = _fixture.Create<string>();
            var apprenticeshipHashedId = _fixture.Create<string>();
            var controller = new ChangeOfPriceEmployerController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, GetUrlBuilder());
            var apprenticeshipKey = _fixture.Create<Guid>();
            _mockApprenticeshipService.Setup(x => x.GetApprenticeshipKey(It.IsAny<string>())).ReturnsAsync(apprenticeshipKey);

            // Act
            var result = await controller.ApproveOrRejectPriceChangePage(employerAccountId, apprenticeshipHashedId, "0", rejectReason);

            // Assert
            _mockApprenticeshipService.Verify(x => x.RejectPendingPriceChange(apprenticeshipKey, expectedEncodedReason), Times.Once);
            result.ShouldBeOfType<RedirectResult>();
            var redirectResult = (RedirectResult)result;
            redirectResult.Url.Should().Be($"https://approvals.at-eas.apprenticeships.education.gov.uk/{employerAccountId}/apprentices/{apprenticeshipHashedId.ToUpper()}/details?showPriceChangeRejected=true");
        }

        [Test]
        public async Task EmployerApproveChange_ApprovePriceHistoryAndRedirectsToEmployerCommitments()
        {
            // Arrange
            var employerAccountId = _fixture.Create<string>();
            var apprenticeshipHashedId = _fixture.Create<string>();
            var controller = new ChangeOfPriceEmployerController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, GetUrlBuilder());
            var apprenticeshipKey = _fixture.Create<Guid>();
            _mockApprenticeshipService.Setup(x => x.GetApprenticeshipKey(It.IsAny<string>())).ReturnsAsync(apprenticeshipKey);
            var userId = _fixture.Create<string>();
            controller.SetupHttpContext(null, null, userId);
            // Act
            var result = await controller.ApproveOrRejectPriceChangePage(employerAccountId, apprenticeshipHashedId, "1", "");

            // Assert
            _mockApprenticeshipService.Verify(x => x.ApprovePendingPriceChange(apprenticeshipKey, userId), Times.Once);
            result.ShouldBeOfType<RedirectResult>();
            var redirectResult = (RedirectResult)result;
            redirectResult.Url.Should().Be($"https://approvals.at-eas.apprenticeships.education.gov.uk/{employerAccountId}/apprentices/{apprenticeshipHashedId.ToUpper()}/details?showPriceChangeApproved=true");
        }

        [Test]
        public async Task CancelPriceChange_CancelTrue_CancelsPriceHistoryAndRedirectsToProviderCommitments()
        {
            // Arrange
            var employerAccountId = _fixture.Create<string>();
            var apprenticeshipHashedId = _fixture.Create<string>();
            var controller = new ChangeOfPriceEmployerController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, GetUrlBuilder());
            var expectedUrl = $"https://approvals.at-eas.apprenticeships.education.gov.uk/{employerAccountId}/apprentices/{apprenticeshipHashedId.ToUpper()}/details?showPriceChangeCancelled=true";
            _mockExternalUrlHelper.Setup(x => x.GenerateUrl(It.IsAny<UrlParameters>())).Returns(expectedUrl);
            var apprenticeshipKey = _fixture.Create<Guid>();
            _mockApprenticeshipService.Setup(x => x.GetApprenticeshipKey(It.IsAny<string>())).ReturnsAsync(apprenticeshipKey);

            // Act
            var result = await controller.CancelPriceChange(employerAccountId, apprenticeshipHashedId, "1");

            // Assert
            _mockApprenticeshipService.Verify(x => x.CancelPendingPriceChange(apprenticeshipKey), Times.Once);
            result.ShouldBeOfType<RedirectResult>();
            ((RedirectResult)result).Url.Should().Be(expectedUrl);
        }

        [Test]
        public async Task CancelPriceChange_CancelFalse_DoesNotCancelPriceHistoryAndRedirectsToProviderCommitments()
        {
            // Arrange
            var employerAccountId = _fixture.Create<string>();
            var apprenticeshipHashedId = _fixture.Create<string>();
            var providerUserName = _fixture.Create<string>();

            var controller = new ChangeOfPriceEmployerController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, GetUrlBuilder());
            var expectedUrl = $"https://approvals.at-eas.apprenticeships.education.gov.uk/{employerAccountId}/apprentices/{apprenticeshipHashedId.ToUpper()}/details";
            _mockExternalUrlHelper.Setup(x => x.GenerateUrl(It.IsAny<UrlParameters>())).Returns(expectedUrl);
            var apprenticeshipKey = _fixture.Create<Guid>();
            _mockApprenticeshipService.Setup(x => x.GetApprenticeshipKey(It.IsAny<string>())).ReturnsAsync(apprenticeshipKey);

            // Act
            var result = await controller.CancelPriceChange(employerAccountId, apprenticeshipHashedId, "0");

            // Assert
            _mockApprenticeshipService.Verify(x => x.CancelPendingPriceChange(apprenticeshipKey), Times.Never);
            result.ShouldBeOfType<RedirectResult>();
            ((RedirectResult)result).Url.Should().Be(expectedUrl);
        }

        private UrlBuilder GetUrlBuilder()
        {
            return new UrlBuilder("AT");
        }
    }
}
