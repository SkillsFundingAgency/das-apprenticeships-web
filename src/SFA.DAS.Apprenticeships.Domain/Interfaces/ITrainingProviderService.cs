namespace SFA.DAS.Apprenticeships.Domain.Interfaces;

/// <summary>
/// Contract to interact with Training Provider(RoATP/APAR) Outer Api.
/// </summary>
public interface ITrainingProviderService
{
    /// <summary>
    /// Contract to get the details of the Provider by given ukprn or provider Id.
    /// </summary>
    /// <param name="ukprn">Provider's ID (UK Provider Reference Number).</param>
    /// <returns>bool.</returns>
    Task<bool> CanProviderAccessService(long ukprn);
}