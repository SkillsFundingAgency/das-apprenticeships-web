using AutoFixture;
using FluentAssertions;
using Moq;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;
using SFA.DAS.Apprenticeships.Web.Models;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfPrice;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Models;

public class MapperResolverTests
{
    private readonly MapperResolver _sut;
    private readonly Fixture _fixture;

    public MapperResolverTests()
    {
        _sut = new MapperResolver();
        _fixture = new Fixture();
    }

    [Test]
    public void Map_WithRegisteredMapper_ShouldMapSuccessfully()
    {
        // Arrange
        var sourceObject = _fixture.Create<ApprenticeshipPrice>();
        var expectedDestinationModel = _fixture.Create<ProviderChangeOfPriceModel>();

        var mapperMock = new Mock<IMapper<ProviderChangeOfPriceModel>>();
        mapperMock.Setup(m => m.Map(sourceObject)).Returns(expectedDestinationModel);

        _sut.Register(mapperMock.Object);

        // Act
        var result = _sut.Map<ProviderChangeOfPriceModel>(sourceObject);

        // Assert
        result.Should().BeEquivalentTo(expectedDestinationModel);
    }
}