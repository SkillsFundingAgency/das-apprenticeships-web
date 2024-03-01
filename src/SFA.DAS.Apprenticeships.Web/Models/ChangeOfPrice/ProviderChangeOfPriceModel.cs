using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;

namespace SFA.DAS.Apprenticeships.Web.Models.ChangeOfPrice;

public class ProviderChangeOfPriceModel : BaseChangeOfPriceModel, IChangeOfPriceModel, ICacheModel, IRouteValuesProvider
{
    public long? ProviderReferenceNumber { get; set; }
    public int ApprenticeshipTrainingPrice { get; set; }
    public int ApprenticeshipEndPointAssessmentPrice { get; set; }
    public InitiatedBy InitiatedBy => InitiatedBy.Provider;
    public int ApprenticeshipTotalPrice => ApprenticeshipTrainingPrice + ApprenticeshipEndPointAssessmentPrice;

    /// <summary>
    /// This is required for validation purposes, and saves making a second call to the API to get the original price
    /// </summary>
    public int OriginalTrainingPrice { get; set; }

    /// <summary>
    /// This is required for validation purposes, and saves making a second call to the API to get the original price
    /// </summary>
    public int OriginalEndPointAssessmentPrice { get; set; }
}

public class CreateChangeOfPriceModelMapper : IMapper<ProviderChangeOfPriceModel>
{
    public ProviderChangeOfPriceModel Map(object sourceObject)
    {
        if(sourceObject is ApprenticeshipPrice apprenticeshipPrice)
        {
            return FromApprenticeshipPrice(apprenticeshipPrice);
        }

        throw new NotImplementedException($"There is not mapping available for object of type {sourceObject.GetType().Name}");
    }

    private static ProviderChangeOfPriceModel FromApprenticeshipPrice(ApprenticeshipPrice apprenticeshipPrice)
    {
        var model = new ProviderChangeOfPriceModel
		{
            FundingBandMaximum = Convert.ToInt32(apprenticeshipPrice.FundingBandMaximum),
            ApprenticeshipTrainingPrice = Convert.ToInt32(apprenticeshipPrice.TrainingPrice),
            ApprenticeshipEndPointAssessmentPrice = Convert.ToInt32(apprenticeshipPrice.AssessmentPrice),
            ApprenticeshipActualStartDate = apprenticeshipPrice.ApprenticeshipActualStartDate,
            ApprenticeshipPlannedEndDate = apprenticeshipPrice.ApprenticeshipPlannedEndDate,
			EarliestEffectiveDate = apprenticeshipPrice.EarliestEffectiveDate,
            ApprovingPartyName = apprenticeshipPrice.EmployerName,
            ApprenticeshipKey = apprenticeshipPrice.ApprenticeshipKey
        };

        model.OriginalTrainingPrice = model.ApprenticeshipTrainingPrice;
        model.OriginalEndPointAssessmentPrice = model.ApprenticeshipEndPointAssessmentPrice;

        return model;
    }
}
