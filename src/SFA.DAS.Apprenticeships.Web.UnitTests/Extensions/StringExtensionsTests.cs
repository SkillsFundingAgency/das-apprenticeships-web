using SFA.DAS.Apprenticeships.Web.Extensions;
using System.Text.Json;
using FluentAssertions;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Extensions;

public class StringExtensionsTests
{
    [Test]
    public void GetFlatJson_ShouldReturnFlatJsonDictionary()
    {
        // Arrange
        var json = @"{
                ""key1"": ""value1"",
                ""key2"": {
                    ""key3"": ""value3"",
                    ""key4"": {
                        ""key5"": ""value5""
                    }
                }
            }";

        var expected = new Dictionary<string, JsonElement>
        {
            {"key1", JsonDocument.Parse("\"value1\"").RootElement},
            {"key2.key3", JsonDocument.Parse("\"value3\"").RootElement},
            {"key2.key4.key5", JsonDocument.Parse("\"value5\"").RootElement}
        };

        // Act
        var result = json.GetFlatJson();

        // Assert
        foreach (var key in expected.Keys)
        {
            result.Should().ContainKey(key);
            result[key].ToString().Should().Be(expected[key].ToString());
        }
    }
}