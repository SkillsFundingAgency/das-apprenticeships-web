namespace SFA.DAS.Apprenticeships.Domain.Interfaces;

public interface IDeleteApiRequest : IApiRequest
{
    /// <summary>
    /// This is the relative URL used in the DELETE request. Note that this should not have a leading slash.
    /// </summary>
    string DeleteUrl { get; }
}