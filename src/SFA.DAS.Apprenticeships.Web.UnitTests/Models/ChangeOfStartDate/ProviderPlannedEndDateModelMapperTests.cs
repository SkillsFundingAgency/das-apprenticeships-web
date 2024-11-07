using AutoFixture;
using FluentAssertions.Execution;
using FluentAssertions;
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
        using (new AssertionScope())
        {
            result.ApprenticeshipKey.Should().Be(apprenticeshipStartDate.ApprenticeshipKey);
            result.ProviderReferenceNumber.Should().Be(apprenticeshipStartDate.ProviderReferenceNumber);
            result.CacheKey.Should().Be(apprenticeshipStartDate.CacheKey);
            result.ApprenticeshipHashedId.Should().Be(apprenticeshipStartDate.ApprenticeshipHashedId);
            result.ApprenticeshipActualStartDate?.Date.Should().Be(apprenticeshipStartDate.ApprenticeshipActualStartDate?.Date);
            result.PlannedEndDate.Should().Be(apprenticeshipStartDate.PlannedEndDate);
            result.OriginalApprenticeshipActualStartDate.Should().Be(apprenticeshipStartDate.OriginalApprenticeshipActualStartDate);
            result.OriginalPlannedEndDate.Should().Be(apprenticeshipStartDate.OriginalPlannedEndDate);
        }
    }

    [Test]
    public void Map_WhenSourceObjectIsNotSupportedForMapping_ThrowsNotImplementedException()
    {
        // Arrange
        var sourceObject = new object();

        // Act & Assert
        _mapper.Invoking(m => m.Map(sourceObject)).Should().Throw<NotImplementedException>();
    }
}
