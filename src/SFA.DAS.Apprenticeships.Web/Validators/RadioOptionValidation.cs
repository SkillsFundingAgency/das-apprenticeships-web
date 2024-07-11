using FluentValidation;
using SFA.DAS.Apprenticeships.Web.Attributes;

namespace SFA.DAS.Apprenticeships.Web.Validators;

public class RadioOptionValidation<T> : AbstractValidator<T>
{
	public RadioOptionValidation()
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
		
		return false;
	}
}
