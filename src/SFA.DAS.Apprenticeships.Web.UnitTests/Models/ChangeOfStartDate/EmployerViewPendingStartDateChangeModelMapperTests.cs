using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Responses;
using SFA.DAS.Apprenticeships.Web.Exceptions;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfStartDate;
using AutoFixture;
using FluentAssertions;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Models.ChangeOfStartDate;

[TestFixture]
public class EmployerViewPendingStartDateChangeModelMapperTests
{
    private Fixture _fixture;
    private EmployerViewPendingStartDateChangeModelMapper _mapper;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _mapper = new EmployerViewPendingStartDateChangeModelMapper();
    }

    [Test]
    public void Map_ThrowsException_WhenSourceObjectIsNotGetPendingStartDateChangeResponse()
    {
        // Arrange
        var sourceObject = new object();

        // Act & Assert
        FluentActions
            .Invoking(() => _mapper.Map(sourceObject))
            .Should()
            .Throw<NotImplementedException>();
    }

    [Test]
    public void Map_ThrowsException_WhenPendingStartDateChangeIsNull()
    {
        // Arrange
        var sourceObject = new GetPendingStartDateChangeResponse();

        // Act & Assert
        FluentActions
            .Invoking(() => _mapper.Map(sourceObject))
            .Should()
            .Throw<MapperException>();
    }

    [Test]
    public void Map_ReturnsCorrectModel_WhenSourceObjectIsValid()
    {
        // Arrange
        var sourceObject = _fixture.Create<GetPendingStartDateChangeResponse>();

        // Act
        var result = _mapper.Map(sourceObject);

        // Assert
        result.ApprenticeshipKey.Should().Be(sourceObject.PendingStartDateChange!.ApprenticeshipKey);
        result.ReasonForChangeOfStartDate.Should().Be(sourceObject.PendingStartDateChange.Reason);
        result.ProviderName.Should().Be(sourceObject.ProviderName);
        result.OriginalActualStartDate.Should().Be(sourceObject.PendingStartDateChange.OriginalActualStartDate);
        result.PendingActualStartDate.Should().Be(sourceObject.PendingStartDateChange.PendingActualStartDate);
        result.OriginalPlannedEndDate.Should().Be(sourceObject.PendingStartDateChange.OriginalPlannedEndDate);
        result.PendingPlannedEndDate.Should().Be(sourceObject.PendingStartDateChange.PendingPlannedEndDate);
    }
}
