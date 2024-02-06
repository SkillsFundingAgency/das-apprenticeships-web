using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;

namespace SFA.DAS.Apprenticeships.Web.Models
{
    public class EmployerChangeOfPriceModel : BaseChangeOfPriceModel, IChangeOfPriceModel, ICacheModel
    {
        public string? EmployerAccountId { get; set; }
        public int ApprenticeshipTotalPrice { get; set; }
        public int OriginalApprenticeshipTotalPrice { get; set; }
        public InitiatedBy InitiatedBy => InitiatedBy.Employer;
    }

    public class EmployerChangeOfPriceModelMapper : IMapper<EmployerChangeOfPriceModel>
    {
        public EmployerChangeOfPriceModel Map(object sourceObject)
        {
            if (sourceObject is ApprenticeshipPrice apprenticeshipPrice)
            {
                return FromApprenticeshipPrice(apprenticeshipPrice);
            }

            throw new NotImplementedException($"There is not mapping available for object of type {sourceObject.GetType().Name}");
        }

        private static EmployerChangeOfPriceModel FromApprenticeshipPrice(ApprenticeshipPrice apprenticeshipPrice)
        {
            var model = new EmployerChangeOfPriceModel
            {
                FundingBandMaximum = Convert.ToInt32(apprenticeshipPrice.FundingBandMaximum),
                ApprenticeshipTotalPrice = Convert.ToInt32(apprenticeshipPrice.TrainingPrice + apprenticeshipPrice.AssessmentPrice),
                ApprenticeshipActualStartDate = apprenticeshipPrice.ApprenticeshipActualStartDate,
                ApprenticeshipPlannedEndDate = apprenticeshipPrice.ApprenticeshipPlannedEndDate,
                EarliestEffectiveDate = apprenticeshipPrice.EarliestEffectiveDate,
                ApprovingPartyName = apprenticeshipPrice.EmployerName
            };

            model.OriginalApprenticeshipTotalPrice = model.ApprenticeshipTotalPrice;

            return model;
        }
    }
}
