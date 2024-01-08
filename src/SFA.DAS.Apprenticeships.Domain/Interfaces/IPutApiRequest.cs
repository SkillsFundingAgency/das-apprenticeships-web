using System.Text.Json.Serialization;

namespace SFA.DAS.Apprenticeships.Domain.Interfaces
{
    public interface IPutApiRequest 
    {
        /// <summary>
        /// This is the relative URL used in the PUT request. Note that this should not have a leading slash.
        /// </summary>
        [JsonIgnore]
        string PutUrl { get; }

        public object Data { get; set; }
    }
}