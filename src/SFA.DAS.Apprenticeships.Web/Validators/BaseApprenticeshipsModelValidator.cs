using FluentValidation;
using SFA.DAS.Apprenticeships.Web.Attributes;

namespace SFA.DAS.Apprenticeships.Web.Validators;

/// <summary>
/// Base validator for models across apprenticeships web, includes validation for radio buttons controlled by marker attributes
/// </summary>
/// <typeparam name="T">Model type</typeparam>
public abstract class BaseApprenticeshipsModelValidator<T> : AbstractValidator<T>
{
	public BaseApprenticeshipsModelValidator()
	{
		var properties = typeof(T).GetProperties()
			.Where(prop => Attribute.IsDefined(prop, typeof(RadioOptionAttribute)));

		foreach (var property in properties)
		{
			var propertyName = property.Name;

			RuleFor(model => property.GetValue(model))
				.Must(x => HaveRadioOptionSelected(x))
				.WithMessage("An option must be selected")
				.WithName(propertyName);
		}
	}

	private static bool HaveRadioOptionSelected(object radioOption)
	{
		if(radioOption == null)
			return false;

		if(radioOption is string radioOptionString)
		{
			if (!string.IsNullOrEmpty(radioOptionString))
				return true;
		}

		bool? radioOptionBool = radioOption as bool?;

        if (radioOptionBool.HasValue)
		{
			return true;
		}

		return false;
	}
}
