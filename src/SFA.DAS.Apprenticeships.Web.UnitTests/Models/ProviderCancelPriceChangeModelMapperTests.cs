﻿using AutoFixture;
using FluentAssertions;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfPrice;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Models
{
    public class ProviderCancelPriceChangeModelMapperTests
    {
        private readonly Fixture _fixture;

        public ProviderCancelPriceChangeModelMapperTests()
        {
            _fixture = new Fixture();
        }

        [Test]
        public void Map_ApprenticeshipPriceToCreateChangeOfPriceModel()
        {
            // Arrange
            var sourceObject = _fixture.Create<GetPendingPriceChangeResponse>();
            var mapper = new ProviderCancelPriceChangeModelMapper();

            // Act
            var result = mapper.Map(sourceObject);

            // Assert
            result.ApprenticeshipKey.Should().Be(sourceObject.PendingPriceChange.ApprenticeshipKey);
            result.ApprenticeshipTrainingPrice.Should().Be(sourceObject.PendingPriceChange.PendingTrainingPrice);
            result.ApprenticeshipEndPointAssessmentPrice.Should().Be(sourceObject.PendingPriceChange.PendingAssessmentPrice);
            result.OriginalTrainingPrice.Should().Be(sourceObject.PendingPriceChange.OriginalTrainingPrice!.Value);
            result.OriginalEndPointAssessmentPrice.Should().Be(sourceObject.PendingPriceChange.OriginalAssessmentPrice!.Value);
            result.EffectiveFromDate.Should().Be(sourceObject.PendingPriceChange.EffectiveFrom);
            result.ReasonForChangeOfPrice.Should().Be(sourceObject.PendingPriceChange.Reason);
        }
    }
}
