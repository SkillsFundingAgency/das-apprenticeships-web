using System.Text.Json.Serialization;

namespace SFA.DAS.Apprenticeships.Domain.Interfaces
{
    public interface IPatchApiRequest
    {
        /// <summary>
        /// This is the relative URL used in the GET request. Note that this should not have a leading slash.
        /// </summary>
        [JsonIgnore]
        string PatchUrl { get; }
        object Data { get; set; }
    }
}