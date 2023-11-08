﻿using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;

namespace SFA.DAS.Apprenticeships.Web.Models
{
    public class CreateChangeOfPriceModel
    {
        public string? ApprenticeshipHashedId { get; set; }
        public string? ProviderReferenceNumber { get; set; }
        public int FundingBandMaximum { get; set; }
        public int ApprenticeshipTrainingPrice { get; set; }
        public int ApprenticeshipEndPointAssessmentPrice { get; set; }
        public int ApprenticeshipTotalPrice => ApprenticeshipTrainingPrice + ApprenticeshipEndPointAssessmentPrice;
    }

    public class CreatChangeOfPriceModelMapper : IMapper<CreateChangeOfPriceModel>
    {
        public CreateChangeOfPriceModel Map(object sourceObject)
        {
            if(sourceObject is ApprenticeshipPrice apprenticeshipPrice)
            {
                return FromApprenticeshipPrice(apprenticeshipPrice);
            }

            throw new NotImplementedException($"There is not mapping available for object of type {sourceObject.GetType().Name}");
        }

        public CreateChangeOfPriceModel FromApprenticeshipPrice(ApprenticeshipPrice apprenticeshipPrice)
        {
            return new CreateChangeOfPriceModel
            {
                FundingBandMaximum = Convert.ToInt32(apprenticeshipPrice.FundingBandMaximum),
                ApprenticeshipTrainingPrice = Convert.ToInt32(apprenticeshipPrice.TrainingPrice),
                ApprenticeshipEndPointAssessmentPrice = Convert.ToInt32(apprenticeshipPrice.AssessmentPrice)
            };
        }
    }
}
