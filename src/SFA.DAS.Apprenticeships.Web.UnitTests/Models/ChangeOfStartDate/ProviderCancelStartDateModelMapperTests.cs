using AutoFixture;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Responses;
using SFA.DAS.Apprenticeships.Web.Exceptions;
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
		Assert.Throws<NotImplementedException>(() => mapper.Map(sourceObject));
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
		Assert.IsInstanceOf<ProviderCancelStartDateModel>(result);
		Assert.That(result.ApprenticeshipKey, Is.EqualTo(sourceObject.PendingStartDateChange!.ApprenticeshipKey));
		Assert.That(result.ReasonForChangeOfStartDate, Is.EqualTo(sourceObject.PendingStartDateChange.Reason));
		Assert.That(result.OriginalStartDate, Is.EqualTo(sourceObject.PendingStartDateChange.OriginalActualStartDate));
		Assert.That(result.PendingStartDate, Is.EqualTo(sourceObject.PendingStartDateChange.PendingActualStartDate));
	}
}
