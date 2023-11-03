using System.Drawing;

namespace SFA.DAS.Apprenticeships.Web.Models
{
    public class CreateChangeOfPriceModel
    {
        public int FundingBandMaximum { get; set; }
        public int ApprenticeshipTrainingPrice { get; set; }
        public int ApprenticeshipEndPointAssessmentPrice { get; set; }
    }
}
