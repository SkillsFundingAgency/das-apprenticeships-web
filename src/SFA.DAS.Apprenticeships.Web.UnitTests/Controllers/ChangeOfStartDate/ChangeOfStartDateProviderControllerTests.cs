using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Apprenticeships.Application.Exceptions;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Responses;
using SFA.DAS.Apprenticeships.Domain.Interfaces;
using SFA.DAS.Apprenticeships.Web.Controllers.ChangeOfStartDate;
using SFA.DAS.Apprenticeships.Web.Models;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfStartDate;
using SFA.DAS.Apprenticeships.Web.Models.Enums;
using SFA.DAS.Apprenticeships.Web.Services;
using SFA.DAS.Apprenticeships.Web.UnitTests.TestHelpers;
using SFA.DAS.Provider.Shared.UI.Extensions;
using SFA.DAS.Provider.Shared.UI.Models;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Controllers.ChangeOfStartDate;

[TestFixture]
public class ChangeOfStartDateProviderControllerTests
{
    private Fixture _fixture;
    private Mock<ILogger<ChangeOfStartDateProviderController>> _mockLogger;
    private Mock<IApprenticeshipService> _mockApprenticeshipService;
    private Mock<IMapper> _mockMapper;
    private Mock<IExternalUrlHelper> _mockExternalUrlHelper;
    private Mock<ICacheService> _mockCacheService;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _mockLogger = new Mock<ILogger<ChangeOfStartDateProviderController>>();
        _mockApprenticeshipService = new Mock<IApprenticeshipService>();
        _mockMapper = new Mock<IMapper>();
        _mockExternalUrlHelper = new Mock<IExternalUrlHelper>();
        _mockCacheService = new Mock<ICacheService>();
    }

    [Test]
    public async Task GetEnterStartDatePage_ReturnsMappedModel()
    {
        // Arrange
        var apprenticeshipHashedId = _fixture.Create<string>();
        var apprenticeshipKey = Guid.NewGuid();
        var expectedModel = _fixture.Create<ProviderChangeOfStartDateModel>();
        var controller = GetSubjectUnderTest();

        _mockMapper.Setup(m => m.Map<ProviderChangeOfStartDateModel>(It.IsAny<ApprenticeshipStartDate>())).Returns(expectedModel);

        var apprenticeshipStartDate = new ApprenticeshipStartDate { ApprenticeshipKey = apprenticeshipKey };
        SetupGetStartDate(apprenticeshipHashedId, apprenticeshipStartDate);

        controller.SetupHttpContext(_fixture.Create<long>(), apprenticeshipHashedId);

        // Act
        var result = await controller.GetEnterStartDatePage(apprenticeshipHashedId);

        // Assert
        var viewResult = result.ShouldBeOfType<ViewResult>();
        viewResult.ViewName.Should().Be(ChangeOfStartDateProviderController.EnterNewStartDateViewName);
        viewResult.Model.Should().BeEquivalentTo(expectedModel);
    }

    [Test]
    public async Task GetEnterStartDatePage_WhenApprenticeshipKeyNotFound_ReturnsNotFound()
    {
        // Arrange
        var apprenticeshipHashedId = _fixture.Create<string>();
		var controller = GetSubjectUnderTest();
		_mockApprenticeshipService.Setup(m => m.GetApprenticeshipKey(apprenticeshipHashedId))
            .ReturnsAsync(Guid.Empty);

        // Act
        var result = await controller.GetEnterStartDatePage(apprenticeshipHashedId);

        // Assert
        result.ShouldBeOfType<NotFoundResult>();
    }

    [Test]
    public async Task GetEnterStartDatePage_WhenApprenticeshipStartDateNotFound_ReturnsNotFound()
    {
        // Arrange
        var apprenticeshipHashedId = _fixture.Create<string>();
        var controller = GetSubjectUnderTest();
        SetupGetStartDate(apprenticeshipHashedId, null);

        // Act
        var result = await controller.GetEnterStartDatePage(apprenticeshipHashedId);

        // Assert
        result.ShouldBeOfType<NotFoundResult>();
    }

    [Test]
    public async Task SubmitStartDate_ValidModel_ReturnsEnterNewEndDateView()
    {
        // Arrange
        var controller = GetSubjectUnderTest();
        var model = _fixture.Create<ProviderChangeOfStartDateModel>();
        var expectedModel = _fixture.Create<ProviderPlannedEndDateModel>();
        _mockMapper.Setup(m => m.Map<ProviderChangeOfStartDateModel>(It.IsAny<ProviderChangeOfStartDateModel>())).Returns(expectedModel);

        controller.SetupHttpContext(_fixture.Create<long>(), _fixture.Create<string>());

        // Act
        var result = await controller.SubmitStartDate(model);

        // Assert
        var viewResult = result.ShouldBeOfType<ViewResult>();
        viewResult.ViewName.Should().Be(ChangeOfStartDateProviderController.EnterNewEndDateViewName);
        viewResult.Model.Should().BeEquivalentTo(expectedModel);
    }

    [Test]
    public async Task SubmitStartDate_InValidModel_ReturnsEnterNewStartDateView()
    {         
        // Arrange
        var controller = GetSubjectUnderTest();
        var model = _fixture.Create<ProviderChangeOfStartDateModel>();

        controller.SetupHttpContext(_fixture.Create<long>(), _fixture.Create<string>());
        controller.ModelState.AddModelError("key", "error message");

        // Act
        var result = await controller.SubmitStartDate(model);

        // Assert
        var viewResult = result.ShouldBeOfType<ViewResult>();
        viewResult.ViewName.Should().Be(ChangeOfStartDateProviderController.EnterNewStartDateViewName);
        viewResult.Model.Should().BeEquivalentTo(model);
    }

    [Test]
    public async Task ProviderCheckDetails_ValidModel_ReturnsCheckDetailsView()
    {
		// Arrange
		var controller = GetSubjectUnderTest();
		_mockMapper.Setup(m => m.Map<ProviderChangeOfStartDateModel>(It.IsAny<ApprenticeshipStartDate>()));

        var model = _fixture.Create<ProviderPlannedEndDateModel>();

        controller.SetupHttpContext(_fixture.Create<long>(), _fixture.Create<string>());

        // Act
        var result = await controller.ProviderCheckDetails(model);

        // Assert
        var viewResult = result.ShouldBeOfType<ViewResult>();
        viewResult.ViewName.Should().Be(ChangeOfStartDateProviderController.CheckDetailsViewName);
        viewResult.Model.Should().BeEquivalentTo(model);
    }

    [Test]
    public async Task ProviderCheckDetails_InValidModel_ReturnsEnterChangeDetailsViewName()
    {
		// Arrange
		var controller = GetSubjectUnderTest();
		_mockMapper.Setup(m => m.Map<ProviderChangeOfStartDateModel>(It.IsAny<ApprenticeshipStartDate>()));

        var model = _fixture.Create<ProviderPlannedEndDateModel>();

        controller.SetupHttpContext(_fixture.Create<long>(), _fixture.Create<string>());
        controller.ModelState.AddModelError("key", "error message");

        // Act
        var result = await controller.ProviderCheckDetails(model);

        // Assert
        var viewResult = result.ShouldBeOfType<ViewResult>();
        viewResult.ViewName.Should().Be(ChangeOfStartDateProviderController.EnterNewEndDateViewName);
        viewResult.Model.Should().BeEquivalentTo(model);
    }

    [Test]
    public async Task ProviderSubmitChangeDetails_ValidModel_Redirects()
    {
		// Arrange
		var controller = GetSubjectUnderTest();
		_mockMapper.Setup(m => m.Map<ProviderChangeOfStartDateModel>(It.IsAny<ApprenticeshipStartDate>()));

        var expectedRedirectUrl = _fixture.Create<string>();
        _mockExternalUrlHelper.Setup(m=>m.GenerateUrl(It.IsAny<UrlParameters>())).Returns(expectedRedirectUrl);
        var model = _fixture.Create<ProviderChangeOfStartDateModel>();

        controller.SetupHttpContext(_fixture.Create<long>(), _fixture.Create<string>(), _fixture.Create<string>());

        // Act
        var result = await controller.ProviderSubmitChangeDetails(model);

        // Assert
        var redirectResult = result.ShouldBeOfType<RedirectResult>();
        redirectResult.Url.Should().Be($"{expectedRedirectUrl}?banners={(ulong)ProviderApprenticeDetailsBanners.ChangeOfStartDateSent}");
    }

	[Test]
	public async Task ViewPendingChangePage_WhenPendingStartDateChangeIsNull_ReturnsNotFound()
	{
        // Arrange
        var controller = GetSubjectUnderTest();

        // Act
        var result = await controller.ViewPendingChangePage(123, "apprenticeshipHashedId");

        // Assert
        result.ShouldBeOfType<NotFoundResult>();
    }

    [Test]
    public async Task ViewPendingChangePage_ProviderInitiated_ReturnsCancelView()
    {
        // Arrange
        var hashId = "hashId";
        var ukprn = _fixture.Create<long>();
        var controller = GetSubjectUnderTest();
        controller.SetupHttpContext(ukprn, hashId);

        var pendingStartDateChangeResponse = _fixture.Create<GetPendingStartDateChangeResponse>();
        pendingStartDateChangeResponse.PendingStartDateChange!.Initiator = "Provider";

        SetupGetPendingStartDateChange(hashId, pendingStartDateChangeResponse);
        _mockMapper.Setup(m => m.Map<ProviderCancelStartDateModel>(It.IsAny<GetPendingStartDateChangeResponse>())).Returns(new ProviderCancelStartDateModel());

        // Act
        var result = await controller.ViewPendingChangePage(ukprn, hashId);

        // Assert
        var viewResult = result.ShouldBeOfType<ViewResult>();
        viewResult.ViewName.Should().Be(ChangeOfStartDateProviderController.ProviderCancelPendingChangeViewName);
    }

    [Test]
    public void ViewPendingChangePage_UnrecognisedInitiator_ThrowsException()
    {
        // Arrange
        var hashId = "hashId";
        var ukprn = _fixture.Create<long>();
        var controller = GetSubjectUnderTest();
        controller.SetupHttpContext(ukprn, hashId);

        var pendingStartDateChangeResponse = _fixture.Create<GetPendingStartDateChangeResponse>();
        pendingStartDateChangeResponse.PendingStartDateChange!.Initiator = "InvalidTestValue";

        SetupGetPendingStartDateChange(hashId, pendingStartDateChangeResponse);
        _mockMapper.Setup(m => m.Map<ProviderCancelStartDateModel>(It.IsAny<GetPendingStartDateChangeResponse>())).Returns(new ProviderCancelStartDateModel());

        // Act & Assert
        FluentActions
            .Invoking(() => controller.ViewPendingChangePage(ukprn, hashId))
            .Should()
            .ThrowAsync<ServiceException>();
    }

    [TestCase("startDate", ChangeOfStartDateProviderController.EnterNewStartDateViewName)]
    [TestCase("endDate", ChangeOfStartDateProviderController.EnterNewEndDateViewName)]
    public void GetProviderInitiatedEditPage_ReturnsExpectedView(string urlQueryParameter, string expectedViewName)
    {
        // Arrange
        var hashId = "hashId";
        var ukprn = _fixture.Create<long>();
        var controller = GetSubjectUnderTest();
        var httpContextMocks = controller.SetupHttpContext(ukprn, hashId);
        httpContextMocks.SetQueryString(new[] { new KeyValuePair<string, string>("view", urlQueryParameter) });
        var createChangeOfStartDateModel = _fixture.Create<ProviderPlannedEndDateModel>();

        // Act
        var result = controller.GetProviderEditChangeDetails(createChangeOfStartDateModel);

        // Assert
        var viewResult = result.ShouldBeOfType<ViewResult>();
        viewResult.ViewName.Should().Be(expectedViewName);
    }

    [Test]
    public void GetProviderInitiatedEditPage_WhenQueryParameterNotPresent_ReturnsNotFound()
    {
        // Arrange
        var hashId = "hashId";
        var ukprn = _fixture.Create<long>();
        var controller = GetSubjectUnderTest();
        var httpContextMocks = controller.SetupHttpContext(ukprn, hashId);
        httpContextMocks.SetQueryString(Array.Empty<KeyValuePair<string, string>>());
        var createChangeOfStartDateModel = _fixture.Create<ProviderPlannedEndDateModel>();

        // Act
        var result = controller.GetProviderEditChangeDetails(createChangeOfStartDateModel);

        // Assert
        result.ShouldBeOfType<NotFoundResult>();
    }

    [Test]
    public async Task CancelStartDateChange_KeepPendingChange_DoesNotCancelChange()
    {
        // Arrange
        var model = _fixture.Create<ProviderCancelStartDateModel>();
        model.CancelRequest = "0";
        var controller = GetSubjectUnderTest();
        controller.SetupHttpContext(model.ProviderReferenceNumber, model.ApprenticeshipHashedId);
        var expectedUrl = _fixture.Create<string>();
        _mockExternalUrlHelper.Setup(x => x.GenerateUrl(It.IsAny<UrlParameters>())).Returns(expectedUrl);

        // Act
        var result = await controller.CancelStartDateChange(model);

        // Assert
        _mockApprenticeshipService.Verify(m => m.CancelPendingStartDateChange(It.IsAny<Guid>()), Times.Never);
        var redirectResult = result.ShouldBeOfType<RedirectResult>();
        redirectResult.Url.Should().Be(expectedUrl);
    }

    [Test]
    public async Task CancelStartDateChange_CancelPendingChange_CancelsChange()
    {
        // Arrange
        var model = _fixture.Create<ProviderCancelStartDateModel>();
        model.CancelRequest = "1";
        var controller = GetSubjectUnderTest();
        controller.SetupHttpContext(model.ProviderReferenceNumber, model.ApprenticeshipHashedId);
        var expectedUrl = _fixture.Create<string>();
        _mockExternalUrlHelper.Setup(x => x.GenerateUrl(It.IsAny<UrlParameters>())).Returns(expectedUrl);

        // Act
        var result = await controller.CancelStartDateChange(model);

        // Assert
        _mockApprenticeshipService.Verify(m => m.CancelPendingStartDateChange(It.IsAny<Guid>()), Times.Once);
        var redirectResult = result.ShouldBeOfType<RedirectResult>();
        redirectResult.Url.Should().Be($"{expectedUrl}?banners={(ulong)ProviderApprenticeDetailsBanners.ChangeOfStartDateCancelled}");
    }

    private ChangeOfStartDateProviderController GetSubjectUnderTest()
    {
        return new ChangeOfStartDateProviderController(
            _mockLogger.Object,
            _mockApprenticeshipService.Object,
            _mockMapper.Object,
            _mockCacheService.Object,
            _mockExternalUrlHelper.Object); 
    }

    private void SetupGetStartDate(string apprenticeshipHashedId, ApprenticeshipStartDate? apprenticeshipStartDate)
    {

        if(apprenticeshipStartDate == null)
            return;

        _mockApprenticeshipService.Setup(m => m.GetApprenticeshipStartDate(apprenticeshipHashedId)).ReturnsAsync(apprenticeshipStartDate);

    }

    private void SetupGetPendingStartDateChange(string apprenticeshipHashedId, GetPendingStartDateChangeResponse? pendingStartDateChangeResponse)
    {
        if(pendingStartDateChangeResponse == null)
            return;

        _mockApprenticeshipService.Setup(m => m.GetPendingStartDateChange(apprenticeshipHashedId)).ReturnsAsync(pendingStartDateChangeResponse);

    }
}