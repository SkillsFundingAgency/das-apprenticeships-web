using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace SFA.DAS.Apprenticeships.Application.UnitTests.TestHelpers;

public static class AssertExtensions
{
	//extension that asserts that the object is of the expected type and returns it
	public static T ShouldBeOfType<T>(this object? actual)
	{
		if (actual == null)
			throw new AssertionException($"Expected object of type {typeof(T).Name} but was null");

		actual.Should().BeOfType<T>();
		return (T)actual;
	}

	//extension asserts logged message on ilogger
	public static void ShouldHaveLoggedMessage<T>(this Mock<ILogger<T>> logger, LogLevel logLevel, string message)
	{
		logger.Verify(
				x => x.Log(
					It.Is<LogLevel>(l => l == logLevel),
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => v.ToString() == message),
					It.IsAny<Exception>(),
					It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
				Times.Once,
				$"Expected {logLevel.ToString()} not logged");
	}
}
