using AutoFixture;
using FluentAssertions;
using Moq;
using SFA.DAS.Apprenticeships.Application.Services;
using SFA.DAS.Apprenticeships.Domain.Api;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Requests;
using SFA.DAS.Apprenticeships.Domain.Interfaces;

namespace SFA.DAS.Apprenticeships.Application.UnitTests.Services
{
    [TestFixture]
    public class ApprenticeshipServiceTests
    {
        private Mock<IApiClient> _apiClientMock;
        private ApprenticeshipService _apprenticeshipService;
        private Fixture _fixture;

        public ApprenticeshipServiceTests()
        {
            _fixture = new Fixture();
        }

        [SetUp]
        public void SetUp()
        {
            _apiClientMock = new Mock<IApiClient>();
            _apprenticeshipService = new ApprenticeshipService(_apiClientMock.Object);
        }

        [Test]
        public async Task GetApprenticeshipKey_WhenCalled_ReturnsApprenticeshipKey()
        {
            // Arrange
            var apprenticeshipHashId = "hashId";
            var expectedKey = _fixture.Create<Guid>();
            var response = new ApiResponse<Guid>(expectedKey, System.Net.HttpStatusCode.Accepted, string.Empty);
            _apiClientMock.Setup(x => x.Get<Guid>(It.IsAny<GetApprenticeshipKeyRequest>())).ReturnsAsync(response);

            // Act
            var result = await _apprenticeshipService.GetApprenticeshipKey(apprenticeshipHashId);

            // Assert
            result.Should().Be(expectedKey);
        }

        [Test]
        public async Task GetApprenticeshipPrice_WhenCalled_ReturnsApprenticeshipPrice()
        {
            // Arrange
            var apprenticeshipKey = _fixture.Create<Guid>();
            var expectedPrice = _fixture.Create<ApprenticeshipPrice>();
            var response = new ApiResponse<ApprenticeshipPrice>( expectedPrice, System.Net.HttpStatusCode.Accepted, string.Empty);
            _apiClientMock.Setup(x => x.Get<ApprenticeshipPrice>(It.IsAny<GetApprenticeshipPriceRequest>())).ReturnsAsync(response);

            // Act
            var result = await _apprenticeshipService.GetApprenticeshipPrice(apprenticeshipKey);

            // Assert
            result.Should().Be(expectedPrice);
        }
    }
}
