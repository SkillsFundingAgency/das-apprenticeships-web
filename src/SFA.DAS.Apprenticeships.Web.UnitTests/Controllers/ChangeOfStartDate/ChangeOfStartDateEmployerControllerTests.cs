using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Responses;
using SFA.DAS.Apprenticeships.Domain.Interfaces;
using SFA.DAS.Apprenticeships.Web.Controllers.ChangeOfStartDate;
using SFA.DAS.Apprenticeships.Web.Models;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfStartDate;
using SFA.DAS.Apprenticeships.Web.UnitTests.TestHelpers;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.Apprenticeships.Web.Models.Enums;
using SFA.DAS.Apprenticeships.Web.Services;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Controllers.ChangeOfStartDate;

[TestFixture]
public class ChangeOfStartDateEmployerControllerTests
{
    private Fixture _fixture;
    private Mock<ILogger<ChangeOfStartDateEmployerController>> _loggerMock;
    private Mock<IApprenticeshipService> _apprenticeshipServiceMock;
    private Mock<IMapper> _mapperMock;
    private Mock<ICacheService> _cacheServiceMock;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _loggerMock = new Mock<ILogger<ChangeOfStartDateEmployerController>>();
        _apprenticeshipServiceMock = new Mock<IApprenticeshipService>();
        _mapperMock = new Mock<IMapper>();
        _cacheServiceMock = new Mock<ICacheService>();
    }

    [Test]
    public async Task ViewPendingChangePage_WhenNoPendingChange_ReturnsNotFound()
    {
        // Arrange
        var controller = new ChangeOfStartDateEmployerController(_loggerMock.Object, _apprenticeshipServiceMock.Object, _mapperMock.Object, GetUrlBuilder(), _cacheServiceMock.Object);

        // Act
        var result = await controller.ViewPendingChangePage("employerAccountId", "apprenticeshipHashedId");

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Test]
    public async Task ViewPendingChangePage_WhenStartDateInitiatedByEmployer_ThrowsNotImplementedException()
    {
        // Arrange
        var pendingStartDateChangeResponse = _fixture.Create<GetPendingStartDateChangeResponse>();
        pendingStartDateChangeResponse.PendingStartDateChange!.Initiator = "Employer";
        MocksSetupGetPendingStartDateApis(pendingStartDateChangeResponse);
        var controller = new ChangeOfStartDateEmployerController(_loggerMock.Object, _apprenticeshipServiceMock.Object, _mapperMock.Object, GetUrlBuilder(), _cacheServiceMock.Object);
        controller.SetupHttpContext(null, "apprenticeshipHashedId", null, "employerAccountId");

        // Act & Assert
        await FluentActions
            .Invoking(() => controller.ViewPendingChangePage("employerAccountId", "apprenticeshipHashedId"))
            .Should()
            .ThrowAsync<NotImplementedException>();

    }

    [Test]
    public async Task ViewPendingChangePage_WhenInitiatorInvalid_ThrowsArgOutOfRangeException()
    {
        // Arrange
        var pendingStartDateChangeResponse = _fixture.Create<GetPendingStartDateChangeResponse>();
        pendingStartDateChangeResponse.PendingStartDateChange!.Initiator = "";
        MocksSetupGetPendingStartDateApis(pendingStartDateChangeResponse);
        var controller = new ChangeOfStartDateEmployerController(_loggerMock.Object, _apprenticeshipServiceMock.Object, _mapperMock.Object, GetUrlBuilder(), _cacheServiceMock.Object);
        controller.SetupHttpContext(null, "apprenticeshipHashedId", null, "employerAccountId");

        // Act & Assert
        await FluentActions
            .Invoking(() => controller.ViewPendingChangePage("employerAccountId", "apprenticeshipHashedId"))
            .Should()
            .ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Test]
    public async Task ViewPendingChangePage_WhenProviderInitiated_ReturnsApproveView()
    {
        // Arrange
        var pendingStartDateChangeResponse = _fixture.Create<GetPendingStartDateChangeResponse>();
        pendingStartDateChangeResponse.PendingStartDateChange!.Initiator = "Provider";

		MocksSetupGetPendingStartDateApis(pendingStartDateChangeResponse);

		_mapperMock.Setup(x => x.Map<EmployerViewPendingStartDateChangeModel>(It.IsAny<object>())).Returns(_fixture.Create<EmployerViewPendingStartDateChangeModel>());

        var controller = new ChangeOfStartDateEmployerController(_loggerMock.Object, _apprenticeshipServiceMock.Object, _mapperMock.Object, GetUrlBuilder(), _cacheServiceMock.Object);
        controller.SetupHttpContext(null, "apprenticeshipHashedId", null, "employerAccountId");

        // Act
        var result = await controller.ViewPendingChangePage("employerAccountId", "apprenticeshipHashedId");

        // Assert
        var viewResult = result.ShouldBeOfType<ViewResult>();
        viewResult.ViewName.Should().Be(ChangeOfStartDateEmployerController.ApproveProviderChangeOfStartDateViewName);
        viewResult.Model.ShouldBeOfType<EmployerViewPendingStartDateChangeModel>();
    }

    [Test]
    public async Task EmployerApproveChange_ApprovesStartDateAndRedirectsToEmployerCommitments()
    {
        // Arrange
        var controller = new ChangeOfStartDateEmployerController(_loggerMock.Object, _apprenticeshipServiceMock.Object, _mapperMock.Object, GetUrlBuilder(), _cacheServiceMock.Object);
        var userId = _fixture.Create<string>();
        controller.SetupHttpContext(null, null, userId);
        var model = _fixture.Create<EmployerViewPendingStartDateChangeModel>();
        model.ApproveRequest = "1";

        // Act
        var result = await controller.ApproveOrRejectStartDateChange(model);

        // Assert
        _apprenticeshipServiceMock.Verify(x => x.ApprovePendingStartDateChange(model.ApprenticeshipKey, userId), Times.Once);
        result.ShouldBeOfType<RedirectResult>();
        var redirectResult = (RedirectResult)result;
        redirectResult.Url.Should().ContainAll(model.EmployerAccountId, model.ApprenticeshipHashedId!.ToUpper(), $"banners={(ulong)EmployerApprenticeDetailsBanners.ChangeOfStartDateApproved}");
    }

    [Test]
    public async Task EmployerRejectChange_RejectsStartDateAndRedirectsToEmployerCommitments()
    {
	    // Arrange
	    var controller = new ChangeOfStartDateEmployerController(_loggerMock.Object, _apprenticeshipServiceMock.Object, _mapperMock.Object, GetUrlBuilder(), _cacheServiceMock.Object);
	    controller.SetupHttpContext(null, null, _fixture.Create<string>());
        var model = _fixture.Create<EmployerViewPendingStartDateChangeModel>();
        model.ApproveRequest = "0";

        // Act
        var result = await controller.ApproveOrRejectStartDateChange(model);

	    // Assert
	    _apprenticeshipServiceMock.Verify(x => x.RejectPendingStartDateChange(model.ApprenticeshipKey, model.RejectReason!), Times.Once);
	    result.ShouldBeOfType<RedirectResult>();
	    var redirectResult = (RedirectResult)result;
	    redirectResult.Url.Should().Be($"https://approvals.at-eas.apprenticeships.education.gov.uk/{model.EmployerAccountId}/apprentices/{model.ApprenticeshipHashedId!.ToUpper()}/details?banners={(ulong)EmployerApprenticeDetailsBanners.ChangeOfStartDateRejected}");
    }

	private void MocksSetupGetPendingStartDateApis(GetPendingStartDateChangeResponse getPendingStartDateChangeResponse)
    {
		_apprenticeshipServiceMock.Setup(x => x.GetPendingStartDateChange(It.IsAny<string>())).ReturnsAsync(getPendingStartDateChangeResponse);
	}

    private static UrlBuilder GetUrlBuilder()
    {
        return new UrlBuilder("AT");
    }
}