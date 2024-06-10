using Microsoft.AspNetCore.Mvc.ModelBinding;
using SFA.DAS.Apprenticeships.Web.Extensions;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Extensions;

public class ModelStateExtensionsTests
{
    [Test]
    public void GetErrorSummary_ShouldReturnConcatenatedErrorMessages()
    {
        // Arrange
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("Error1", "This is error 1");
        modelState.AddModelError("Error2", "This is error 2");

        // Act
        var result = modelState.GetErrorSummary();

        // Assert
        Assert.That(result.Equals("This is error 1 This is error 2"));
    }
}
