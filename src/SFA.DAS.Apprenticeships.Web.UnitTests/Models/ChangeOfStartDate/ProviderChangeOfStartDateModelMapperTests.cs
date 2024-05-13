using AutoFixture;
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
        Assert.That(result.ApprenticeshipActualStartDate?.Date, Is.EqualTo(apprenticeshipStartDate.ActualStartDate.Value.Date));
        Assert.That(result.ApprenticeshipKey, Is.EqualTo(apprenticeshipStartDate.ApprenticeshipKey));
        Assert.That(result.EarliestStartDate, Is.EqualTo(apprenticeshipStartDate.EarliestStartDate));
        Assert.That(result.LatestStartDate, Is.EqualTo(apprenticeshipStartDate.LatestStartDate));
        Assert.That(result.LastFridayOfSchool, Is.EqualTo(apprenticeshipStartDate.LastFridayOfSchool));
        Assert.That(result.ApprovingPartyName, Is.EqualTo(apprenticeshipStartDate.EmployerName));
        Assert.That(result.OriginalApprenticeshipActualStartDate, Is.EqualTo(apprenticeshipStartDate.ActualStartDate.Value.Date));
        Assert.That(result.OriginalPlannedEndDate, Is.EqualTo(apprenticeshipStartDate.PlannedEndDate!.Value.Date));
        Assert.That(result.PreviousAcademicYearEndDate, Is.EqualTo(apprenticeshipStartDate.PreviousAcademicYear.EndDate));
        Assert.That(result.PreviousAcademicYearHardCloseDate, Is.EqualTo(apprenticeshipStartDate.PreviousAcademicYear.HardCloseDate));
        Assert.That(result.CurrentAcademicYearStartDate, Is.EqualTo(apprenticeshipStartDate.CurrentAcademicYear.StartDate));
    }

    [Test]
    public void Map_WhenSourceObjectIsNotApprenticeshipStartDate_ThrowsNotImplementedException()
    {
        // Arrange
        var sourceObject = new object();

        // Act & Assert
        Assert.Throws<NotImplementedException>(() => _mapper.Map(sourceObject));
    }
}
