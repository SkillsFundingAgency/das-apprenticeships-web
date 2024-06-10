using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Apprenticeships.Domain.Interfaces;
using SFA.DAS.Apprenticeships.Web.Controllers.ChangeOfPaymentStatus;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfPaymentStatus;
using SFA.DAS.Apprenticeships.Web.Services;
using SFA.DAS.Apprenticeships.Web.UnitTests.TestHelpers;
using SFA.DAS.Employer.Shared.UI;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Controllers.ChangeOfPaymentStatus;

[TestFixture]
public class ChangeOfPaymentStatusEmployerControllerTests
{
    private readonly Fixture _fixture;
    private readonly Mock<ILogger<ChangeOfPaymentStatusEmployerController>> _mockLogger;
    private Mock<IApprenticeshipService> _mockApprenticeshipService = null!;
    private Mock<ICacheService> _mockCacheService;
    private string _expectedEmployerCommitmentsUrl = null!;

    public ChangeOfPaymentStatusEmployerControllerTests()
    {
        _fixture = new Fixture();
        _mockLogger = new Mock<ILogger<ChangeOfPaymentStatusEmployerController>>();
        _mockCacheService = new Mock<ICacheService>();
    }

    [SetUp]
    public void Setup()
    {
        _mockApprenticeshipService = new Mock<IApprenticeshipService>();
        _expectedEmployerCommitmentsUrl = _fixture.Create<string>();
        _mockCacheService = new Mock<ICacheService>();
    }

    [Test]
    public async Task FreezeProviderPaymentsPage_Get_ReturnsCorrectModel()
    {
        // Arrange
        var employerAccountId = _fixture.Create<string>();
        var apprenticeshipHashedId = _fixture.Create<string>();
        var apprenticeshipKey = _fixture.Create<Guid>();

        _mockApprenticeshipService.Setup(m => m.GetApprenticeshipKey(apprenticeshipHashedId)).ReturnsAsync(apprenticeshipKey);

        var controller = new ChangeOfPaymentStatusEmployerController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockCacheService.Object, GetUrlBuilder());
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

        var controller = new ChangeOfPaymentStatusEmployerController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockCacheService.Object, GetUrlBuilder());

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

        var controller = new ChangeOfPaymentStatusEmployerController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockCacheService.Object, GetUrlBuilder());

        // Act
        var result = await controller.FreezeProviderPaymentsPage(model);

        // Assert
        _mockApprenticeshipService.Verify(x => x.FreezePayments(model.ApprenticeshipKey, model.ReasonForFreeze), Times.Once);
        result.ShouldBeOfType<RedirectResult>();
        ((RedirectResult)result).Url.Should().Be($"https://approvals.at-eas.apprenticeships.education.gov.uk/{model.EmployerAccountId}/apprentices/{model.ApprenticeshipHashedId.ToUpper()}/details?showProviderPaymentsInactive=true");
    }

    [Test]
    public async Task UnfreezeProviderPaymentsPage_Get_ReturnsCorrectModel()
    {
        // Arrange
        var employerAccountId = _fixture.Create<string>();
        var apprenticeshipHashedId = _fixture.Create<string>();
        var apprenticeshipKey = _fixture.Create<Guid>();

        _mockApprenticeshipService.Setup(m => m.GetApprenticeshipKey(apprenticeshipHashedId)).ReturnsAsync(apprenticeshipKey);

        var controller = new ChangeOfPaymentStatusEmployerController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockCacheService.Object, GetUrlBuilder());
        controller.SetupHttpContext(null, apprenticeshipHashedId, null, employerAccountId);

        // Act
        var result = await controller.UnfreezeProviderPaymentsPage(employerAccountId, apprenticeshipHashedId);

        // Assert
        var viewResult = result.ShouldBeOfType<ViewResult>();
        var viewModel = viewResult.Model.ShouldBeOfType<UnfreezeProviderPaymentsModel>();
        viewModel.ApprenticeshipHashedId.Should().Be(apprenticeshipHashedId);
        viewModel.ApprenticeshipKey.Should().Be(apprenticeshipKey);
        viewModel.BackLinkUrl.Should().Be($"https://approvals.at-eas.apprenticeships.education.gov.uk/{employerAccountId}/apprentices/{apprenticeshipHashedId.ToUpper()}/details");
        viewModel.EmployerAccountId.Should().Be(employerAccountId);
    }

    [Test]
    public async Task UnfreezeProviderPaymentsPage_Get_ApprenticeshipKeyNotFound_Returns404()
    {
        // Arrange
        var apprenticeshipHashedId = _fixture.Create<string>();
        var employerAccountId = _fixture.Create<string>();

        _mockApprenticeshipService.Setup(m => m.GetApprenticeshipKey(apprenticeshipHashedId)).ReturnsAsync(Guid.Empty);

        var controller = new ChangeOfPaymentStatusEmployerController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockCacheService.Object, GetUrlBuilder());

        // Act
        var result = await controller.UnfreezeProviderPaymentsPage(employerAccountId, apprenticeshipHashedId);

        // Assert
        result.ShouldBeOfType<NotFoundResult>();
    }

    [Test]
    public async Task UnfreezeProviderPaymentsPage_Post_ValidModel_FreezesPaymentsAndRedirectsBackToEmployerCommitments()
    {
        // Arrange
        var model = _fixture.Create<UnfreezeProviderPaymentsModel>();
        model.UnfreezePayments = true;

        var controller = new ChangeOfPaymentStatusEmployerController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockCacheService.Object, GetUrlBuilder());

        // Act
        var result = await controller.UnfreezeProviderPaymentsPage(model);

        // Assert
        _mockApprenticeshipService.Verify(x => x.UnfreezePayments(model.ApprenticeshipKey), Times.Once);
        result.ShouldBeOfType<RedirectResult>();
        ((RedirectResult)result).Url.Should().Be($"https://approvals.at-eas.apprenticeships.education.gov.uk/{model.EmployerAccountId}/apprentices/{model.ApprenticeshipHashedId.ToUpper()}/details?showProviderPaymentsActive=true");
    }

    private UrlBuilder GetUrlBuilder()
    {
        return new UrlBuilder("AT");
    }
}