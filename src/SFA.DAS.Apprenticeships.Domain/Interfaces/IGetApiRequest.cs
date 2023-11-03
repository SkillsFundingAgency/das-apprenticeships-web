using System.Text.Json.Serialization;

namespace SFA.DAS.Apprenticeships.Domain.Interfaces
{
    public interface IGetApiRequest 
    {
        [JsonIgnore]
        string GetUrl { get; }
    }
}