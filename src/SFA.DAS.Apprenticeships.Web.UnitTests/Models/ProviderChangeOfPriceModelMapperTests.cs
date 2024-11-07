using AutoFixture;
using FluentAssertions;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfPrice;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Models;

public class ProviderChangeOfPriceModelMapperTests
{
    [Test]
    public void Map_ApprenticeshipPriceToCreateChangeOfPriceModel()
    {
        // Arrange
        var apprenticeshipPrice = new ApprenticeshipPrice
        {
            TrainingPrice = 16000,
            AssessmentPrice = 2000,
            FundingBandMaximum = 25000,
        };
        var mapper = new ProviderChangeOfPriceModelMapper();

        // Act
        var result = mapper.Map(apprenticeshipPrice);

        // Assert
        result.ApprenticeshipTrainingPrice.Should().Be(16000);
        result.ApprenticeshipEndPointAssessmentPrice.Should().Be(2000);
        result.FundingBandMaximum.Should().Be(25000);
    }
}