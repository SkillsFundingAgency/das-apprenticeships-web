using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Apprenticeships.Domain.Interfaces;
using SFA.DAS.Apprenticeships.Web.Controllers.PaymentsFreeze;
using SFA.DAS.Apprenticeships.Web.Models.PaymentsFreeze;
using SFA.DAS.Apprenticeships.Web.UnitTests.TestHelpers;
using SFA.DAS.Employer.Shared.UI;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Controllers.PaymentsFreeze
{
    [TestFixture]
    public class EmployerPaymentsFreezeControllerTests
    {
        private readonly Fixture _fixture;
        private readonly Mock<ILogger<EmployerPaymentsFreezeController>> _mockLogger;
        private Mock<IApprenticeshipService> _mockApprenticeshipService = null!; // should be initialized in Setup()
        private Mock<UrlBuilder> _mockExternalEmployerUrlHelper = null!;
        private string _expectedEmployerCommitmentsUrl = null!;

        public EmployerPaymentsFreezeControllerTests()
        {
            _fixture = new Fixture();
            _mockLogger = new Mock<ILogger<EmployerPaymentsFreezeController>>();
        }

        [SetUp]
        public void Setup()
        {
            _mockApprenticeshipService = new Mock<IApprenticeshipService>();
            _mockExternalEmployerUrlHelper = new Mock<UrlBuilder>();
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
            _mockExternalEmployerUrlHelper.Setup(x => x.CommitmentsV2Link("ApprenticeDetails", employerAccountId, apprenticeshipHashedId.ToUpper()))
                .Returns(_expectedEmployerCommitmentsUrl);

            var controller = new EmployerPaymentsFreezeController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockExternalEmployerUrlHelper.Object);
            controller.SetupHttpContext(null, apprenticeshipHashedId, null, employerAccountId);

            // Act
            var result = await controller.FreezeProviderPaymentsPage(employerAccountId, apprenticeshipHashedId);

            // Assert
            var viewResult = result.ShouldBeOfType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeOfType<FreezeProviderPaymentsModel>();
            viewModel.ApprenticeshipHashedId.Should().Be(apprenticeshipHashedId);
            viewModel.ApprenticeshipKey.Should().Be(apprenticeshipKey);
            viewModel.BackLinkUrl.Should().Be(_expectedEmployerCommitmentsUrl);
            viewModel.EmployerAccountId.Should().Be(employerAccountId);
        }

        [Test]
        public async Task FreezeProviderPaymentsPage_Get_ApprenticeshipKeyNotFound_Returns404()
        {
            // Arrange
            var apprenticeshipHashedId = _fixture.Create<string>();
            var employerAccountId = _fixture.Create<string>();

            _mockApprenticeshipService.Setup(m => m.GetApprenticeshipKey(apprenticeshipHashedId)).ReturnsAsync(Guid.Empty);

            var controller = new EmployerPaymentsFreezeController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockExternalEmployerUrlHelper.Object);

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
            var expectedUrl = $"{_expectedEmployerCommitmentsUrl}?showProviderPaymentsInactive=true";

            _mockExternalEmployerUrlHelper.Setup(x => x.CommitmentsV2Link("ApprenticeDetails", model.EmployerAccountId, model.ApprenticeshipHashedId.ToUpper()))
                .Returns(_expectedEmployerCommitmentsUrl);

            var controller = new EmployerPaymentsFreezeController(_mockLogger.Object, _mockApprenticeshipService.Object, _mockExternalEmployerUrlHelper.Object);

            // Act
            var result = await controller.FreezeProviderPaymentsPage(model);

            // Assert
            _mockApprenticeshipService.Verify(x => x.FreezePayments(model.ApprenticeshipKey, model.ReasonForFreeze), Times.Once);
            result.ShouldBeOfType<RedirectResult>();
            ((RedirectResult)result).Url.Should().Be(expectedUrl);
        }
    }
}
