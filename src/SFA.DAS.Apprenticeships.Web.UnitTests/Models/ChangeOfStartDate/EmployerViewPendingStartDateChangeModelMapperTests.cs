using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Responses;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;
using SFA.DAS.Apprenticeships.Web.Exceptions;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfStartDate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;

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
        Assert.Throws<NotImplementedException>(() => _mapper.Map(sourceObject));
    }

    [Test]
    public void Map_ThrowsException_WhenPendingStartDateChangeIsNull()
    {
        // Arrange
        var sourceObject = new GetPendingStartDateChangeResponse();

        // Act & Assert
        Assert.Throws<MapperException>(() => _mapper.Map(sourceObject));
    }

    [Test]
    public void Map_ReturnsCorrectModel_WhenSourceObjectIsValid()
    {
        // Arrange
        var sourceObject = _fixture.Create<GetPendingStartDateChangeResponse>();

        // Act
        var result = _mapper.Map(sourceObject);

        // Assert
        Assert.That(result.ApprenticeshipKey, Is.EqualTo(sourceObject.PendingStartDateChange!.ApprenticeshipKey));
        Assert.That(result.ReasonForChangeOfStartDate, Is.EqualTo(sourceObject.PendingStartDateChange.Reason));
        Assert.That(result.ProviderName, Is.EqualTo(sourceObject.ProviderName));
        Assert.That(result.OriginalActualStartDate, Is.EqualTo(sourceObject.PendingStartDateChange.OriginalActualStartDate));
        Assert.That(result.PendingActualStartDate, Is.EqualTo(sourceObject.PendingStartDateChange.PendingActualStartDate));
        Assert.That(result.OriginalPlannedEndDate, Is.EqualTo(sourceObject.PendingStartDateChange.OriginalPlannedEndDate));
        Assert.That(result.PendingPlannedEndDate, Is.EqualTo(sourceObject.PendingStartDateChange.PendingPlannedEndDate));
    }
}
