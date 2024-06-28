using System.Text.Json.Serialization;

namespace SFA.DAS.Apprenticeships.Domain.Interfaces;

public interface IPostApiRequest : IApiRequest
{
    /// <summary>
    /// This is the relative URL used in the POST request. Note that this should not have a leading slash.
    /// </summary>
    [JsonIgnore]
    string PostUrl { get; }

    public object Data { get; set; }
}