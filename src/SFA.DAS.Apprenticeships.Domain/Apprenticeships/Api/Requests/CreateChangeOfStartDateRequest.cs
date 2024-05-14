using SFA.DAS.Apprenticeships.Domain.Interfaces;

namespace SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Requests
{
    public class CreateChangeOfStartDateRequest : IPostApiRequest
    {
        private readonly Guid _apprenticeshipKey;

        public CreateChangeOfStartDateRequest(Guid apprenticeshipKey, CreateChangeOfStartDateData data)
        {
            _apprenticeshipKey = apprenticeshipKey;
            Data = data;
        }

        public string PostUrl => $"Apprenticeship/{_apprenticeshipKey}/startDateChange";
        public bool SendBearerToken => true;
        public object Data { get; set; }
    }

    public class CreateChangeOfStartDateData
    {
        public string Initiator { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public DateTime ActualStartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
    }
}
