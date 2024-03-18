using FluentValidation;
using SFA.DAS.Apprenticeships.Web.Models;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfPrice;

namespace SFA.DAS.Apprenticeships.Web.Validators.ChangeOfPrice
{
    public abstract class BaseChangeOfPriceValidator<T> : AbstractValidator<T>
    {
        protected bool IsValidDate(DateField dateField)
        {
            if (dateField.Date == null)
            {
                return false;
            }

            return true;
        }

        protected bool MustBeAfterTrainingStartDate(BaseChangeOfPriceModel model)
        {
            if (model.ApprenticeshipActualStartDate.HasValue && model.EffectiveFromDate.Date <= model.ApprenticeshipActualStartDate)
            {
                return false;
            }

            return true;
        }

        protected bool MustBeBeforePlannedEndDate(BaseChangeOfPriceModel model)
        {
            if (model.ApprenticeshipPlannedEndDate.HasValue && model.EffectiveFromDate.Date >= model.ApprenticeshipPlannedEndDate)
            {
                return false;
            }

            return true;
        }

        protected bool MustBeAfterEarliestValidDate(BaseChangeOfPriceModel model)
        {
            if (!model.EarliestEffectiveDate.HasValue)
            {
                throw new InvalidOperationException("EarliestEffectiveDate must be set");//This should come from api call to get Apprenticeship Price
            }

            if (model.EffectiveFromDate.Date < model.EarliestEffectiveDate)
            {
                return false;
            }

            return true;
        }

        protected bool MustNotBeInTheFuture(BaseChangeOfPriceModel model)
        {

            if (model.EffectiveFromDate.Date > DateTime.Now.Date)
            {
                return false;
            }

            return true;
        }
    }
}
