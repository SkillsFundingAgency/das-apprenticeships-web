using AutoFixture;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfStartDate;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Models.ChangeOfStartDate;

[TestFixture]
public class ProviderPlannedEndDateModelMapperTests
{
    private ProviderPlannedEndDateModelMapper _mapper;
    private Fixture _fixture;

    [SetUp]
    public void SetUp()
    {
        _mapper = new ProviderPlannedEndDateModelMapper();
        _fixture = new Fixture();
    }

    [Test]
    public void Map_WhenSourceObjectIsProviderChangeOfStartDateModel_ReturnsCorrectModel()
    {
        // Arrange
        var apprenticeshipStartDate = _fixture.Create<ProviderChangeOfStartDateModel>();


        // Act
        var result = _mapper.Map(apprenticeshipStartDate);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.ApprenticeshipKey, Is.EqualTo(apprenticeshipStartDate.ApprenticeshipKey));
            Assert.That(result.ProviderReferenceNumber, Is.EqualTo(apprenticeshipStartDate.ProviderReferenceNumber));
            Assert.That(result.CacheKey, Is.EqualTo(apprenticeshipStartDate.CacheKey));
            Assert.That(result.ApprenticeshipHashedId, Is.EqualTo(apprenticeshipStartDate.ApprenticeshipHashedId));
            Assert.That(result.ApprenticeshipActualStartDate?.Date, Is.EqualTo(apprenticeshipStartDate.ApprenticeshipActualStartDate!.Date));
            Assert.That(result.PlannedEndDate, Is.EqualTo(apprenticeshipStartDate.PlannedEndDate));
            Assert.That(result.OriginalApprenticeshipActualStartDate, Is.EqualTo(apprenticeshipStartDate.OriginalApprenticeshipActualStartDate));
            Assert.That(result.OriginalPlannedEndDate, Is.EqualTo(apprenticeshipStartDate.OriginalPlannedEndDate));
        });
    }

    [Test]
    public void Map_WhenSourceObjectIsNotSupportedForMapping_ThrowsNotImplementedException()
    {
        // Arrange
        var sourceObject = new object();

        // Act & Assert
        Assert.Throws<NotImplementedException>(() => _mapper.Map(sourceObject));
    }
}
