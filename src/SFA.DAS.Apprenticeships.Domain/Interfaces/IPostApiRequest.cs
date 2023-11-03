using System.Text.Json.Serialization;

namespace SFA.DAS.Apprenticeships.Domain.Interfaces
{
    public interface IPostApiRequest 
    {
        [JsonIgnore]
        string PostUrl { get; }

        public object Data { get; set; }
    }
}