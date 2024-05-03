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

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Controllers.ChangeOfStartDate;


[TestFixture]
public class ChangeOfStartDateEmployerControllerTests
{
    private Fixture _fixture;
    private Mock<ILogger<ChangeOfStartDateEmployerController>> _loggerMock;
    private Mock<IApprenticeshipService> _apprenticeshipServiceMock;
    private Mock<IMapper> _mapperMock;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _loggerMock = new Mock<ILogger<ChangeOfStartDateEmployerController>>();
        _apprenticeshipServiceMock = new Mock<IApprenticeshipService>();
        _mapperMock = new Mock<IMapper>();
    }

    [Test]
    public async Task ViewPendingChangePage_WhenNoPendingChange_ReturnsNotFound()
    {
        // Arrange
        var controller = new ChangeOfStartDateEmployerController(_loggerMock.Object, _apprenticeshipServiceMock.Object, _mapperMock.Object, GetUrlBuilder());

        // Act
        var result = await controller.ViewPendingChangePage("employerAccountId", "apprenticeshipHashedId");

        // Assert
        Assert.IsInstanceOf<NotFoundResult>(result);
    }

    [Test]
    public async Task ViewPendingChangePage_WhenProviderInitiated_ReturnsApproveView()
    {
        // Arrange
        var pendingStartDateChangeResponse = _fixture.Create<GetPendingStartDateChangeResponse>();
        pendingStartDateChangeResponse.PendingStartDateChange!.Initiator = "Provider";

		MocksSetupGetPendingStartDateApis(pendingStartDateChangeResponse);

		_mapperMock.Setup(x => x.Map<EmployerViewPendingStartDateChangeModel>(It.IsAny<object>())).Returns(_fixture.Create<EmployerViewPendingStartDateChangeModel>());

        var controller = new ChangeOfStartDateEmployerController(_loggerMock.Object, _apprenticeshipServiceMock.Object, _mapperMock.Object, GetUrlBuilder());
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
        var employerAccountId = _fixture.Create<string>();
        var apprenticeshipHashedId = _fixture.Create<string>();
        var controller = new ChangeOfStartDateEmployerController(_loggerMock.Object, _apprenticeshipServiceMock.Object, _mapperMock.Object, GetUrlBuilder());
        var apprenticeshipKey = _fixture.Create<Guid>();
        _apprenticeshipServiceMock.Setup(x => x.GetApprenticeshipKey(It.IsAny<string>())).ReturnsAsync(apprenticeshipKey);
        var userId = _fixture.Create<string>();
        controller.SetupHttpContext(null, null, userId);
        // Act
        var result = await controller.ApproveOrRejectStartDateChange(employerAccountId, apprenticeshipHashedId, "1", "");

        // Assert
        _apprenticeshipServiceMock.Verify(x => x.ApprovePendingStartDateChange(apprenticeshipKey, userId), Times.Once);
        result.ShouldBeOfType<RedirectResult>();
        var redirectResult = (RedirectResult)result;
        redirectResult.Url.Should().Be($"https://approvals.at-eas.apprenticeships.education.gov.uk/{employerAccountId}/apprentices/{apprenticeshipHashedId.ToUpper()}/details?showStartDateChangeApproved=true");
    }

    [Test]
    public async Task EmployerRejectChange_RejectsStartDateAndRedirectsToEmployerCommitments()
    {
	    // Arrange
	    var employerAccountId = _fixture.Create<string>();
	    var apprenticeshipHashedId = _fixture.Create<string>();
	    var controller = new ChangeOfStartDateEmployerController(_loggerMock.Object, _apprenticeshipServiceMock.Object, _mapperMock.Object, GetUrlBuilder());
	    var apprenticeshipKey = _fixture.Create<Guid>();
	    var rejectReason = _fixture.Create<string>();
	    _apprenticeshipServiceMock.Setup(x => x.GetApprenticeshipKey(It.IsAny<string>())).ReturnsAsync(apprenticeshipKey);
	    var userId = _fixture.Create<string>();
	    controller.SetupHttpContext(null, null, userId);
	    // Act
	    var result = await controller.ApproveOrRejectStartDateChange(employerAccountId, apprenticeshipHashedId, "0", rejectReason);

	    // Assert
	    _apprenticeshipServiceMock.Verify(x => x.RejectPendingStartDateChange(apprenticeshipKey, rejectReason), Times.Once);
	    result.ShouldBeOfType<RedirectResult>();
	    var redirectResult = (RedirectResult)result;
	    redirectResult.Url.Should().Be($"https://approvals.at-eas.apprenticeships.education.gov.uk/{employerAccountId}/apprentices/{apprenticeshipHashedId.ToUpper()}/details?showStartDateChangeRejected=true");
    }

	private void MocksSetupGetPendingStartDateApis(GetPendingStartDateChangeResponse getPendingStartDateChangeResponse)
    {
		_apprenticeshipServiceMock.Setup(x => x.GetApprenticeshipKey(It.IsAny<string>())).ReturnsAsync(Guid.NewGuid);
		_apprenticeshipServiceMock.Setup(x => x.GetPendingStartDateChange(It.IsAny<Guid>())).ReturnsAsync(getPendingStartDateChangeResponse);
	}

    private static UrlBuilder GetUrlBuilder()
    {
        return new UrlBuilder("AT");
    }
}

