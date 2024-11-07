using AutoFixture;
using FluentAssertions;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfStartDate;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Models.ChangeOfStartDate;

[TestFixture]
public class ProviderChangeOfStartDateModelMapperTests
{
    private ProviderChangeOfStartDateModelMapper _mapper;
    private Fixture _fixture;

    [SetUp]
    public void SetUp()
    {
        _mapper = new ProviderChangeOfStartDateModelMapper();
        _fixture = new Fixture();
    }

    [Test]
    public void Map_WhenSourceObjectIsApprenticeshipStartDate_ReturnsCorrectModel()
    {
        // Arrange
        var apprenticeshipStartDate = new ApprenticeshipStartDate
        {
            ActualStartDate = DateTime.Now,
            PlannedEndDate = DateTime.Now.AddDays(100),
            ApprenticeshipKey = Guid.NewGuid(),
            EarliestStartDate = DateTime.Now.AddDays(-10),
            LatestStartDate = DateTime.Now.AddDays(10),
            LastFridayOfSchool = DateTime.Now.AddDays(-5),
            EmployerName = "Test Employer",
            Standard = _fixture.Create<StandardInfo>(),
            CurrentAcademicYear = _fixture.Create<AcademicYearDetails>(),
            PreviousAcademicYear = _fixture.Create<AcademicYearDetails>()
        };

        // Act
        var result = _mapper.Map(apprenticeshipStartDate);

        // Assert
        result.ApprenticeshipActualStartDate?.Date.Should().Be(apprenticeshipStartDate.ActualStartDate!.Value.Date);
        result.ApprenticeshipKey.Should().Be(apprenticeshipStartDate.ApprenticeshipKey);
        result.EarliestStartDate.Should().Be(apprenticeshipStartDate.EarliestStartDate);
        result.LatestStartDate.Should().Be(apprenticeshipStartDate.LatestStartDate);
        result.LastFridayOfSchool.Should().Be(apprenticeshipStartDate.LastFridayOfSchool);
        result.ApprovingPartyName.Should().Be(apprenticeshipStartDate.EmployerName);
        result.OriginalApprenticeshipActualStartDate.Should().Be(apprenticeshipStartDate.ActualStartDate!.Value);
        result.OriginalPlannedEndDate.Should().Be(apprenticeshipStartDate.PlannedEndDate!.Value);
        result.PreviousAcademicYearEndDate.Should().Be(apprenticeshipStartDate.PreviousAcademicYear.EndDate);
        result.PreviousAcademicYearHardCloseDate.Should().Be(apprenticeshipStartDate.PreviousAcademicYear.HardCloseDate);
        result.CurrentAcademicYearStartDate.Should().Be(apprenticeshipStartDate.CurrentAcademicYear.StartDate);
    }

    [Test]
    public void Map_WhenSourceObjectIsNotApprenticeshipStartDate_ThrowsNotImplementedException()
    {
        // Arrange
        var sourceObject = new object();

        // Act & Assert
        FluentActions
            .Invoking(() => _mapper.Map(sourceObject))
            .Should()
            .Throw<NotImplementedException>();
    }
}
