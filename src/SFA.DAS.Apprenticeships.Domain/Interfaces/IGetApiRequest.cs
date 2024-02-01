using System.Text.Json.Serialization;

namespace SFA.DAS.Apprenticeships.Domain.Interfaces
{
    public interface IGetApiRequest 
    {
        /// <summary>
        /// This is the relative URL used in the GET request. Note that this should not have a leading slash.
        /// </summary>
        [JsonIgnore]
        string GetUrl { get; }
    }
}