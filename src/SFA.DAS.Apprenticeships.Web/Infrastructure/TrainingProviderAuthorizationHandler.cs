using Microsoft.AspNetCore.Authorization;
using SFA.DAS.Apprenticeships.Domain.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Apprenticeships.Web.Infrastructure
{
    /// <summary>
    /// Interface to define contracts related to Training Provider Authorization Handlers.
    /// </summary>
    public interface ITrainingProviderAuthorizationHandler
    {
        /// <summary>
        /// Contract to check if the authenticated Provider is a valid Training Provider.
        /// </summary>
        /// <param name="context">AuthorizationHandlerContext</param>
        /// <returns>true if the ukprn of the user's claim is associated with a valid training provider with access to the service; otherwise, false.</returns>
        Task<bool> IsProviderAuthorized(AuthorizationHandlerContext context);
    }

    ///<inheritdoc cref="ITrainingProviderAuthorizationHandler"/>
    [ExcludeFromCodeCoverage]
    public class TrainingProviderAuthorizationHandler : ITrainingProviderAuthorizationHandler
    {
        private readonly ITrainingProviderService _trainingProviderService;

        public TrainingProviderAuthorizationHandler(ITrainingProviderService trainingProviderService)
        {
            _trainingProviderService = trainingProviderService;
        }

        public async Task<bool> IsProviderAuthorized(AuthorizationHandlerContext context)
        {
            var ukprn = GetProviderId(context);

            if (ukprn <= 0)
            {
                return false;
            }

            var providerDetails = await _trainingProviderService.CanProviderAccessService(ukprn);

            return providerDetails;
        }

        private static long GetProviderId(AuthorizationHandlerContext authorizationHandlerContext)
        {
            return long.TryParse(authorizationHandlerContext.User.FindFirst(c => c.Type.Equals(ProviderClaims.ProviderUkprn))?.Value, out var providerId) 
                ? providerId 
                : 0;
        }
    }
}
