using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;
using SFA.DAS.Apprenticeships.Domain.Interfaces;
using SFA.DAS.Apprenticeships.Web.Controllers;
using SFA.DAS.Apprenticeships.Web.Models;
using SFA.DAS.Apprenticeships.Web.UnitTests.TestHelpers;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Controllers
{
    [TestFixture]
    public class ChangeOfPriceControllerTests
    {
        private readonly Fixture _fixture;
        private readonly ILogger<ChangeOfPriceController> _logger;
        private Mock<IApprenticeshipService> _mockApprenticeshipService = null!; // should be initialized in Setup()
        private Mock<IMapper<CreateChangeOfPriceModel>> _mockMapper = null!; // should be initialized in Setup()

        public ChangeOfPriceControllerTests()
        {
            _fixture = new Fixture();
            _logger = Mock.Of<ILogger<ChangeOfPriceController>>();
        }

        [SetUp]
        public void Setup()
        {
            _mockApprenticeshipService = new Mock<IApprenticeshipService>();
            _mockMapper = new Mock<IMapper<CreateChangeOfPriceModel>>();
        }

        [Test]
        public async Task GetPage_ReturnsMappedModel()
        {
            // Arrange
            var apprenticeshipPrice = _fixture.Create<ApprenticeshipPrice>();
            _mockApprenticeshipService.Setup(m => m.GetApprenticeshipPrice(It.IsAny<string>())).ReturnsAsync(apprenticeshipPrice);

            var createChangeOfPriceModel = _fixture.Create<CreateChangeOfPriceModel>();
            _mockMapper.Setup(m => m.Map(apprenticeshipPrice)).Returns(createChangeOfPriceModel);

            var controller = new ChangeOfPriceController(_logger, _mockApprenticeshipService.Object, _mockMapper.Object);

            // Act
            var result = await controller.GetPage("anyApprenticeshipId");

            // Assert
            var viewResult = result.ShouldBeOfType<ViewResult>();
            viewResult.Model.Should().Be(createChangeOfPriceModel);
        }

        [Test]
        public void CreatePriceChangeRequest_InvalidModel_ReturnsCreatePriceChangeRequestView()
        {
            // Arrange
            var createChangeOfPriceModel = _fixture.Create<CreateChangeOfPriceModel>();
            var controller = new ChangeOfPriceController(_logger, _mockApprenticeshipService.Object, _mockMapper.Object);
            controller.ModelState.AddModelError("anyKey", "anyErrorMessage");

            // Act
            var result = controller.CreatePriceChangeRequest(createChangeOfPriceModel);

            // Assert
            var viewResult = result.ShouldBeOfType<ViewResult>();
            viewResult.ViewName.Should().Be(ChangeOfPriceController.ChangeOfPriceRequestViewName);
        }
    }
}
