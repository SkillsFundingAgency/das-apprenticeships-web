using AutoFixture;
using FluentAssertions;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfPrice;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Models
{
    public class EmployerChangeOfPriceModelMapperTests
    {
        private readonly Fixture _fixture;

        public EmployerChangeOfPriceModelMapperTests()
        {
            _fixture = new Fixture();
        }

        [Test]
        public void Map_ApprenticeshipPriceToCreateChangeOfPriceModel()
        {
            // Arrange
            var apprenticeshipPrice = _fixture.Create<ApprenticeshipPrice>();
            var mapper = new EmployerChangeOfPriceModelMapper();

            // Act
            var result = mapper.Map(apprenticeshipPrice);

            // Assert
            result.FundingBandMaximum.Should().Be(Convert.ToInt32(apprenticeshipPrice.FundingBandMaximum));
            result.ApprenticeshipTotalPrice.Should().Be(Convert.ToInt32(apprenticeshipPrice.TrainingPrice + apprenticeshipPrice.AssessmentPrice));
            result.ApprenticeshipActualStartDate.Should().Be(apprenticeshipPrice.ApprenticeshipActualStartDate);
            result.ApprenticeshipPlannedEndDate.Should().Be(apprenticeshipPrice.ApprenticeshipPlannedEndDate);
            result.EarliestEffectiveDate.Should().Be(apprenticeshipPrice.EarliestEffectiveDate);
            result.ApprovingPartyName.Should().Be(apprenticeshipPrice.ProviderName);
            result.ApprenticeshipKey.Should().Be(apprenticeshipPrice.ApprenticeshipKey);
            result.OriginalApprenticeshipTotalPrice.Should().Be(Convert.ToInt32(apprenticeshipPrice.TrainingPrice + apprenticeshipPrice.AssessmentPrice));
        }
    }
}
