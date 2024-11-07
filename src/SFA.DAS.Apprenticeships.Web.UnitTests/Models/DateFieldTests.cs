using FluentAssertions;
using SFA.DAS.Apprenticeships.Web.Models;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Models;

public class DateFieldTests
{
    [Test]
    public void Date_ShouldReturnNull_WhenDayMonthYearAreNull()
    {
        var dateField = new DateField { Day = null, Month = null, Year = null };
        dateField.Date.Should().BeNull();
    }

    [Test]
    public void Date_ShouldReturnNull_WhenDateIsInvalid()
    {
        var dateField = new DateField { Day = 31, Month = 2, Year = 2020 }; // No 31st Feb
        dateField.Date.Should().BeNull();
    }

    [Test]
    public void Date_ShouldReturnDate_WhenDateIsValid()
    {
        var dateField = new DateField { Day = 1, Month = 1, Year = 2020 };
        dateField.Date.Should().Be(new DateTime(2020, 1, 1));
    }

    [Test]
    public void ToString_ShouldReturnEmptyString_WhenDateIsNull()
    {
        var dateField = new DateField { Day = null, Month = null, Year = null };
        dateField.ToString().Should().Be(string.Empty);
    }

    [Test]
    public void ToString_ShouldReturnFormattedDate_WhenDateIsValid()
    {
        var dateField = new DateField { Day = 1, Month = 1, Year = 2020 };
        dateField.ToString().Should().Be("01 January 2020");
    }
}