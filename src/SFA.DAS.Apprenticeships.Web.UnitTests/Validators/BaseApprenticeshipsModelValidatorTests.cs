using FluentAssertions;
using SFA.DAS.Apprenticeships.Web.Attributes;
using SFA.DAS.Apprenticeships.Web.Validators;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Validators;

public class BaseApprenticeshipsModelValidatorTests
{
    private SampleValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new SampleValidator();
    }

    [Test]
    public void Should_Have_Error_When_Option1_Is_Null()
    {
        var model = new SampleModel { Option1 = null, Option2 = "Selected" };
        var result = _validator.Validate(model);
        result.Errors.Should().Contain(x => x.PropertyName == "Option1" && x.ErrorMessage == "An option must be selected");
    }

    [Test]
    public void Should_Have_Error_When_Option1_Is_Empty()
    {
        var model = new SampleModel { Option1 = "", Option2 = "Selected" };
        var result = _validator.Validate(model);
        result.Errors.Should().Contain(x => x.PropertyName == "Option1" && x.ErrorMessage == "An option must be selected");
    }

    [Test]
    public void Should_Not_Have_Error_When_Option1_Is_Selected()
    {
        var model = new SampleModel { Option1 = "Selected", Option2 = "Selected" };
        var result = _validator.Validate(model);
        result.Errors.Should().BeEmpty();
    }

    [Test]
    public void Should_Not_Have_Error_For_Non_RadioOption_Properties()
    {
        var model = new SampleModel { Option1 = "Selected", Option2 = "Selected", Option3 = null };
        var result = _validator.Validate(model);
        result.Errors.Should().BeEmpty();
    }
}

public class SampleValidator : BaseApprenticeshipsModelValidator<SampleModel>
{

}

public class SampleModel
{
    [RadioOption]
    public string? Option1 { get; set; }

    [RadioOption]
    public string? Option2 { get; set; }

    public string? Option3 { get; set; }
}