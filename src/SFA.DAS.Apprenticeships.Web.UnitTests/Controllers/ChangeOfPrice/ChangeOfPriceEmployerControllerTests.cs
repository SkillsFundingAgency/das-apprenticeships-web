using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Apprenticeships.Application.Exceptions;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Responses;
using SFA.DAS.Apprenticeships.Domain.Interfaces;
using SFA.DAS.Apprenticeships.Web.Controllers.ChangeOfPrice;
using SFA.DAS.Apprenticeships.Web.Models;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfPrice;
using SFA.DAS.Apprenticeships.Web.Services;
using SFA.DAS.Apprenticeships.Web.UnitTests.TestHelpers;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.Apprenticeships.Web.Models.Enums;
using SFA.DAS.Provider.Shared.UI.Extensions;
using SFA.DAS.Provider.Shared.UI.Models;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Controllers.ChangeOfPrice;

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
        var apprenticeshipPrice = _fixture.Create<ApprenticeshipPrice>();
        _mockApprenticeshipService.Setup(m => m.GetApprenticeshipPrice(apprenticeshipHashedId)).ReturnsAsync(apprenticeshipPrice);

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
        ((RedirectResult)result).Url.Should().EndWith($"?banners={(ulong)EmployerApprenticeDetailsBanners.ChangeOfPriceRequestSent}");
    }

    [Test]
    public async Task GetViewPendingPriceChangePageEmployer_ReturnsCorrectView()
    {
        // Arrange
        var accountId = _fixture.Create<string>();
        var apprenticeshipHashedId = _fixture.Create<string>();
        var employerAccountId = _fixture.Create<string>();
        var pendingPriceChange = _fixture.Create<GetPendingPriceChangeResponse>();
        var viewModel = _fixture.Create<EmployerCancelPriceChangeModel>();
        pendingPriceChange.HasPendingPriceChange = true;
        pendingPriceChange.PendingPriceChange.Initiator = "Employer";

        _mockApprenticeshipService.Setup(x => x.GetPendingPriceChange(apprenticeshipHashedId))
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

    [Test]
    public async Task GetViewPendingPriceChangePageEmployer_ProviderInitiated_ReturnsCorrectView()
    {
        // Arrange
        var accountId = _fixture.Create<string>();
        var apprenticeshipHashedId = _fixture.Create<string>();
        var employerAccountId = _fixture.Create<string>();
        var pendingPriceChange = _fixture.Create<GetPendingPriceChangeResponse>();
        var viewModel = _fixture.Create<EmployerViewPendingPriceChangeModel>();
        pendingPriceChange.HasPendingPriceChange = true;
        pendingPriceChange.PendingPriceChange.Initiator = "Provider";

        _mockApprenticeshipService.Setup(x => x.GetPendingPriceChange(apprenticeshipHashedId))
            .ReturnsAsync(pendingPriceChange);

        _mockMapper.Setup(x => x.Map<EmployerViewPendingPriceChangeModel>(pendingPriceChange))
            .Returns(viewModel);

        var controller = new ChangeOfPriceEmployerController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, GetUrlBuilder());
        controller.SetupHttpContext(null, apprenticeshipHashedId, null, employerAccountId);

        // Act
        var result = await controller.ViewPendingPriceChangePage(accountId, apprenticeshipHashedId);

        // Assert
        var viewResult = result.ShouldBeOfType<ViewResult>();
        viewResult.ViewName.Should().Be(ChangeOfPriceEmployerController.ApproveProviderChangeOfPriceViewName);
        viewResult.Model.ShouldBeOfType<EmployerViewPendingPriceChangeModel>();
    }

    [Test]
    public void GetViewPendingPriceChangePageEmployer_UnrecognisedInitiator_ThrowsException()
    {
        // Arrange
        var accountId = _fixture.Create<string>();
        var apprenticeshipHashedId = _fixture.Create<string>();
        var employerAccountId = _fixture.Create<string>();
        var pendingPriceChange = _fixture.Create<GetPendingPriceChangeResponse>();
        var viewModel = _fixture.Create<EmployerViewPendingPriceChangeModel>();
        pendingPriceChange.HasPendingPriceChange = true;
        pendingPriceChange.PendingPriceChange.Initiator = "InvalidTestValue";

        _mockApprenticeshipService.Setup(x => x.GetPendingPriceChange(apprenticeshipHashedId))
            .ReturnsAsync(pendingPriceChange);

        _mockMapper.Setup(x => x.Map<EmployerViewPendingPriceChangeModel>(pendingPriceChange))
            .Returns(viewModel);

        var controller = new ChangeOfPriceEmployerController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, GetUrlBuilder());
        controller.SetupHttpContext(null, apprenticeshipHashedId, null, employerAccountId);

        // Act & Assert
        FluentActions
            .Invoking(() => controller.ViewPendingPriceChangePage(accountId, apprenticeshipHashedId))
            .Should()
            .ThrowAsync<ServiceException>();
    }

    [Test]
    public async Task GetViewPendingPriceChangePageEmployer_ReturnsNotFoundWhenNoPriceChangeExists()
    {
        // Arrange
        var accountId = _fixture.Create<string>();
        var apprenticeshipHashedId = _fixture.Create<string>();
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
    public async Task EmployerRejectChange_ApproveFalse_RejectsPriceHistoryAndRedirectsToEmployerCommitments(string? rejectReason, string expectedEncodedReason)
    {
        // Arrange
        var employerAccountId = _fixture.Create<string>();
        var apprenticeshipHashedId = _fixture.Create<string>();
        var controller = new ChangeOfPriceEmployerController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, GetUrlBuilder());
        var apprenticeshipKey = _fixture.Create<Guid>();
        _mockApprenticeshipService.Setup(x => x.GetApprenticeshipKey(It.IsAny<string>())).ReturnsAsync(apprenticeshipKey);
        var model = new EmployerViewPendingPriceChangeModel 
        {
            EmployerAccountId = employerAccountId,
            ApprenticeshipHashedId=apprenticeshipHashedId,
            ApproveChanges = "0",
            RejectReason = rejectReason
        };

        // Act
        var result = await controller.ApproveOrRejectPriceChangePage(model);

        // Assert
        _mockApprenticeshipService.Verify(x => x.RejectPendingPriceChange(apprenticeshipKey, expectedEncodedReason), Times.Once);
        result.ShouldBeOfType<RedirectResult>();
        var redirectResult = (RedirectResult)result;
        redirectResult.Url.Should().Be($"https://approvals.at-eas.apprenticeships.education.gov.uk/{employerAccountId}/apprentices/{apprenticeshipHashedId.ToUpper()}/details?banners={(ulong)EmployerApprenticeDetailsBanners.ChangeOfPriceRejected}");
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
        var model = new EmployerViewPendingPriceChangeModel
        {
            EmployerAccountId = employerAccountId,
            ApprenticeshipHashedId = apprenticeshipHashedId,
            ApproveChanges = "1"
        };

        // Act
        var result = await controller.ApproveOrRejectPriceChangePage(model);

        // Assert
        _mockApprenticeshipService.Verify(x => x.ApprovePendingPriceChange(apprenticeshipKey, userId), Times.Once);
        result.ShouldBeOfType<RedirectResult>();
        var redirectResult = (RedirectResult)result;
        redirectResult.Url.Should().Be($"https://approvals.at-eas.apprenticeships.education.gov.uk/{employerAccountId}/apprentices/{apprenticeshipHashedId.ToUpper()}/details?banners={(ulong)EmployerApprenticeDetailsBanners.ChangeOfPriceApproved}");
    }

    [Test]
    public async Task ApproveOrRejectPriceChange_ApprenticeshipKeyNotFound_ReturnsNotFound()
    {
        // Arrange
        var controller = new ChangeOfPriceEmployerController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, GetUrlBuilder());
        _mockApprenticeshipService.Setup(x => x.GetApprenticeshipKey(It.IsAny<string>())).ReturnsAsync(Guid.Empty);

        // Act
        var result = await controller.ApproveOrRejectPriceChangePage(_fixture.Create<EmployerViewPendingPriceChangeModel>());

        // Assert
        result.ShouldBeOfType<NotFoundResult>();
    }

    [Test]
    public async Task CancelPriceChange_CancelTrue_CancelsPriceHistoryAndRedirectsToProviderCommitments()
    {
        // Arrange
        var employerAccountId = _fixture.Create<string>();
        var apprenticeshipHashedId = _fixture.Create<string>();
        var controller = new ChangeOfPriceEmployerController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, GetUrlBuilder());
        var expectedUrl = $"https://approvals.at-eas.apprenticeships.education.gov.uk/{employerAccountId}/apprentices/{apprenticeshipHashedId.ToUpper()}/details";
        _mockExternalUrlHelper.Setup(x => x.GenerateUrl(It.IsAny<UrlParameters>())).Returns(expectedUrl);
        var apprenticeshipKey = _fixture.Create<Guid>();
        _mockApprenticeshipService.Setup(x => x.GetApprenticeshipKey(It.IsAny<string>())).ReturnsAsync(apprenticeshipKey);

        // Act
        var result = await controller.CancelPriceChange(employerAccountId, apprenticeshipHashedId, "1");

        // Assert
        _mockApprenticeshipService.Verify(x => x.CancelPendingPriceChange(apprenticeshipKey), Times.Once);
        result.ShouldBeOfType<RedirectResult>();
        ((RedirectResult)result).Url.Should().Be($"{expectedUrl}?banners={(ulong)EmployerApprenticeDetailsBanners.ChangeOfPriceCancelled}");
    }

    [Test]
    public async Task CancelPriceChange_CancelFalse_DoesNotCancelPriceHistoryAndRedirectsToProviderCommitments()
    {
        // Arrange
        var employerAccountId = _fixture.Create<string>();
        var apprenticeshipHashedId = _fixture.Create<string>();

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

    [Test]
    public async Task CancelPriceChange_ApprenticeshipKeyNotFound_ReturnsNotFound()
    {
        // Arrange
        var controller = new ChangeOfPriceEmployerController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, GetUrlBuilder());
        _mockApprenticeshipService.Setup(x => x.GetApprenticeshipKey(It.IsAny<string>())).ReturnsAsync(Guid.Empty);

        // Act
        var result = await controller.CancelPriceChange(_fixture.Create<string>(), _fixture.Create<string>(), "1");

        // Assert
        result.ShouldBeOfType<NotFoundResult>();
    }

    [Test]
    public void GetEmployerEditChangeDetails_ReturnsViewWithModel()
    {
        // Arrange
        var model = _fixture.Create<EmployerChangeOfPriceModel>();
        var controller = new ChangeOfPriceEmployerController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, GetUrlBuilder());

        // Act
        var result = controller.GetEmployerEditChangeDetails(model);

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewName.Should().Be(ChangeOfPriceEmployerController.EnterChangeDetailsViewName);
        viewResult.Model.Should().Be(model);
    }

    [Test]
    public async Task EmployerCheckDetails_InvalidModel_ReturnsViewWithModel()
    {
        // Arrange
        var model = _fixture.Create<EmployerChangeOfPriceModel>();
        var controller = new ChangeOfPriceEmployerController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, GetUrlBuilder());
        controller.ModelState.AddModelError("PropertyName", "Error Message");

        // Act
        var result = await controller.EmployerCheckDetails(model);

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewName.Should().Be(ChangeOfPriceEmployerController.EnterChangeDetailsViewName);
        viewResult.Model.Should().Be(model);
    }

    [Test]
    public async Task EmployerCheckDetails_ValidModel_SetsCacheAndReturnsView()
    {
        // Arrange
        var model = _fixture.Create<EmployerChangeOfPriceModel>();
        var controller = new ChangeOfPriceEmployerController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockMapper.Object, _mockCacheService.Object, GetUrlBuilder());

        // Act
        var result = await controller.EmployerCheckDetails(model);

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewName.Should().Be(ChangeOfPriceEmployerController.CheckDetailsViewName);
        viewResult.Model.Should().Be(model);

        _mockCacheService.Verify(x => x.SetCacheModelAsync(model), Times.Once);
    }

    private static UrlBuilder GetUrlBuilder()
    {
        return new UrlBuilder("AT");
    }
}