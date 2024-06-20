using SFA.DAS.Apprenticeships.Web.Models.ChangeOfStartDate;
using SFA.DAS.Apprenticeships.Web.Models;
using SFA.DAS.Apprenticeships.Web.Validators.ChangeOfStartDate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Web.Validators.PaymentsFreeze;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfPaymentStatus;
using AutoFixture;
using FluentAssertions;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Validators.PaymentsFreeze;

public class UnfreezeProviderPaymentsModelValidatorTests
{
    private Fixture _fixture;

    public UnfreezeProviderPaymentsModelValidatorTests()
    {
        _fixture = new Fixture();    
    }

    [Test]
    public void Validate_Valid_DoesNotReturnMessage()
    {
        // Arrange
        var model = BuildValidTestModel();
        var validator = new UnfreezeProviderPaymentsModelValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void Validate_UnfreezePaymentsBooleanNull_FailsValidation()
    {
        // Arrange
        var model = BuildValidTestModel();
        model.UnfreezePayments = null;
        var validator = new UnfreezeProviderPaymentsModelValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.ErrorMessage == "You must select an option");
    }

    private UnfreezeProviderPaymentsModel BuildValidTestModel()
    {
        var model = _fixture.Create<UnfreezeProviderPaymentsModel>();
        return model;
    }
}
