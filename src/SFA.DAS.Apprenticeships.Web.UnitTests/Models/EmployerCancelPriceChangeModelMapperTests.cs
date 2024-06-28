using AutoFixture;
using FluentAssertions;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Responses;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfPrice;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Models;

public class EmployerCancelPriceChangeModelMapperTests
{
    private readonly Fixture _fixture;

    public EmployerCancelPriceChangeModelMapperTests()
    {
        _fixture = new Fixture();
    }

    [Test]
    public void Map_ApprenticeshipPriceToCreateChangeOfPriceModel()
    {
        // Arrange
        var sourceObject = _fixture.Create<GetPendingPriceChangeResponse>();
        var mapper = new EmployerCancelPriceChangeModelMapper();

        // Act
        var result = mapper.Map(sourceObject);

        // Assert
        result.ApprenticeshipKey.Should().Be(sourceObject.PendingPriceChange.ApprenticeshipKey);
        result.EffectiveFromDate.Should().Be(sourceObject.PendingPriceChange.EffectiveFrom);
        result.ReasonForChangeOfPrice.Should().Be(sourceObject.PendingPriceChange.Reason);
        result.ProviderName.Should().Be(sourceObject.ProviderName);
        result.FirstName.Should().Be(sourceObject.PendingPriceChange.FirstName);
        result.LastName.Should().Be(sourceObject.PendingPriceChange.LastName);
    }
}