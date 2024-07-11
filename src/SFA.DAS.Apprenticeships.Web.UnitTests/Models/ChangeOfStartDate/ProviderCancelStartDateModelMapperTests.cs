using AutoFixture;
using FluentAssertions;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Responses;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfStartDate;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Models.ChangeOfStartDate;

[TestFixture]
public class ProviderCancelStartDateModelMapperTests
{
	private Fixture _fixture;

	[SetUp]
	public void SetUp()
	{
		_fixture = new Fixture();
	}

	[Test]
	public void Map_WhenSourceHasNoMapping_ThrowsNotImplementedException()
	{
		var sourceObject = new object();
		var mapper = new ProviderCancelStartDateModelMapper();
		
        FluentActions
            .Invoking(() => mapper.Map(sourceObject))
            .Should()
            .Throw<NotImplementedException>();
    }

	[Test]
	public void Map_WhenSourceValid_ReturnsModel()
	{
		// Arrange
		var sourceObject = _fixture.Create<GetPendingStartDateChangeResponse>();
		var mapper = new ProviderCancelStartDateModelMapper();

		// Act
		var result = mapper.Map(sourceObject);

        // Assert
        result.Should().BeOfType<ProviderCancelStartDateModel>();
        result.ApprenticeshipKey.Should().Be(sourceObject.PendingStartDateChange!.ApprenticeshipKey);
        result.ReasonForChangeOfStartDate.Should().Be(sourceObject.PendingStartDateChange.Reason);
        result.OriginalStartDate.Should().Be(sourceObject.PendingStartDateChange.OriginalActualStartDate);
        result.PendingStartDate.Should().Be(sourceObject.PendingStartDateChange.PendingActualStartDate);
		result.OriginalPlannedEndDate.Should().Be(sourceObject.PendingStartDateChange.OriginalPlannedEndDate);
		result.PendingPlannedEndDate.Should().Be(sourceObject.PendingStartDateChange.PendingPlannedEndDate);

	}
}
