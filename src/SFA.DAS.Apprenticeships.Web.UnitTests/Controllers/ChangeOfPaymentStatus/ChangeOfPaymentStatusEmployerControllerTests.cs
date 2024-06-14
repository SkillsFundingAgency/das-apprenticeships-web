using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Apprenticeships.Domain.Interfaces;
using SFA.DAS.Apprenticeships.Web.Controllers.ChangeOfPaymentStatus;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfPaymentStatus;
using SFA.DAS.Apprenticeships.Web.UnitTests.TestHelpers;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.Employer.Shared.UI.Models.Flags;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Controllers.ChangeOfPaymentStatus;

[TestFixture]
public class ChangeOfPaymentStatusEmployerControllerTests
{
    private readonly Fixture _fixture;
    private readonly Mock<ILogger<ChangeOfPaymentStatusEmployerController>> _mockLogger;
    private Mock<IApprenticeshipService> _mockApprenticeshipService = null!;
    private string _expectedEmployerCommitmentsUrl = null!;

    public ChangeOfPaymentStatusEmployerControllerTests()
    {
        _fixture = new Fixture();
        _mockLogger = new Mock<ILogger<ChangeOfPaymentStatusEmployerController>>();
    }

    [SetUp]
    public void Setup()
    {
        _mockApprenticeshipService = new Mock<IApprenticeshipService>();
        _expectedEmployerCommitmentsUrl = _fixture.Create<string>();
    }

    [Test]
    public async Task FreezeProviderPaymentsPage_Get_ReturnsCorrectModel()
    {
        // Arrange
        var employerAccountId = _fixture.Create<string>();
        var apprenticeshipHashedId = _fixture.Create<string>();
        var apprenticeshipKey = _fixture.Create<Guid>();

        _mockApprenticeshipService.Setup(m => m.GetApprenticeshipKey(apprenticeshipHashedId)).ReturnsAsync(apprenticeshipKey);

        var controller = new ChangeOfPaymentStatusEmployerController(_mockLogger.Object, _mockApprenticeshipService.Object, GetUrlBuilder());
        controller.SetupHttpContext(null, apprenticeshipHashedId, null, employerAccountId);

        // Act
        var result = await controller.FreezeProviderPaymentsPage(employerAccountId, apprenticeshipHashedId);

        // Assert
        var viewResult = result.ShouldBeOfType<ViewResult>();
        var viewModel = viewResult.Model.ShouldBeOfType<FreezeProviderPaymentsModel>();
        viewModel.ApprenticeshipHashedId.Should().Be(apprenticeshipHashedId);
        viewModel.ApprenticeshipKey.Should().Be(apprenticeshipKey);
        viewModel.BackLinkUrl.Should().Be($"https://approvals.at-eas.apprenticeships.education.gov.uk/{employerAccountId}/apprentices/{apprenticeshipHashedId.ToUpper()}/details");
        viewModel.EmployerAccountId.Should().Be(employerAccountId);
    }

    [Test]
    public async Task FreezeProviderPaymentsPage_Get_ApprenticeshipKeyNotFound_Returns404()
    {
        // Arrange
        var apprenticeshipHashedId = _fixture.Create<string>();
        var employerAccountId = _fixture.Create<string>();

        _mockApprenticeshipService.Setup(m => m.GetApprenticeshipKey(apprenticeshipHashedId)).ReturnsAsync(Guid.Empty);

        var controller = new ChangeOfPaymentStatusEmployerController(_mockLogger.Object, _mockApprenticeshipService.Object, GetUrlBuilder());

        // Act
        var result = await controller.FreezeProviderPaymentsPage(employerAccountId, apprenticeshipHashedId);

        // Assert
        result.ShouldBeOfType<NotFoundResult>();
    }

    [Test]
    public async Task FreezeProviderPaymentsPage_Post_ValidModel_FreezesPaymentsAndRedirectsBackToEmployerCommitments()
    {
        // Arrange
        var model = _fixture.Create<FreezeProviderPaymentsModel>();

        var controller = new ChangeOfPaymentStatusEmployerController(_mockLogger.Object, _mockApprenticeshipService.Object, GetUrlBuilder());

        // Act
        var result = await controller.FreezeProviderPaymentsPage(model);

        // Assert
        _mockApprenticeshipService.Verify(x => x.FreezePayments(model.ApprenticeshipKey, model.ReasonForFreeze), Times.Once);
        result.ShouldBeOfType<RedirectResult>();
        ((RedirectResult)result).Url.Should().Be($"https://approvals.at-eas.apprenticeships.education.gov.uk/{model.EmployerAccountId}/apprentices/{model.ApprenticeshipHashedId.ToUpper()}/details?banners={ApprenticeDetailsBanners.ProviderPaymentsInactive}");
    }

    private UrlBuilder GetUrlBuilder()
    {
        return new UrlBuilder("AT");
    }
}