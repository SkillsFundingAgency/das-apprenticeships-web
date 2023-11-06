using FluentAssertions;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.TestHelpers
{
    public static class AssertExtensions
    {
        //extension that asserts that the object is of the expected type and returns it
        public static T ShouldBeOfType<T>(this object actual)
        {
            actual.Should().BeOfType<T>();
            return (T)actual;
        }
    }
}
