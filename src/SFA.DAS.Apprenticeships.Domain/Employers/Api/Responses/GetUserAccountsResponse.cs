using System.Text.Json.Serialization;

namespace SFA.DAS.Apprenticeships.Domain.Employers.Api.Responses;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public class GetUserAccountsResponse
{
    [JsonPropertyName("isSuspended")]
    public bool IsSuspended { get; set; }

    [JsonPropertyName("lastName")]
    public string LastName { get; set; }

    [JsonPropertyName("firstName")]
    public string FirstName { get; set; }

    [JsonPropertyName("employerUserId")]
    public string EmployerUserId { get; set; }

    [JsonPropertyName("userAccounts")]
    public List<EmployerIdentifier> UserAccounts { get; set; }
}

public class EmployerIdentifier
{
    [JsonPropertyName("encodedAccountId")]
    public string AccountId { get; set; }
    [JsonPropertyName("dasAccountName")]
    public string EmployerName { get; set; }
    [JsonPropertyName("role")]
    public string Role { get; set; }

}