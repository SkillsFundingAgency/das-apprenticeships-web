using Newtonsoft.Json;

namespace SFA.DAS.Apprenticeships.Domain.Employers.Api.Responses
{
    public class GetUserAccountsResponse
    {
        [JsonProperty(nameof(IsSuspended))]  
        public bool IsSuspended { get; set; }
        [JsonProperty(nameof(UserAccounts))]
        public List<EmployerIdentifier> UserAccounts { get; set; }
    }
    
    public class EmployerIdentifier
    {
        [JsonProperty("EncodedAccountId")]
        public string AccountId { get; set; }
        [JsonProperty("DasAccountName")]
        public string EmployerName { get; set; }
        [JsonProperty(nameof(Role))]
        public string Role { get; set; }
    }
}