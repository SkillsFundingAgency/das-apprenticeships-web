using System.Text.Json.Serialization;

namespace SFA.DAS.Apprenticeships.Domain.Interfaces
{
    public interface IPutApiRequest 
    {
        [JsonIgnore]
        string PutUrl { get; }

        public object Data { get; set; }
    }
}