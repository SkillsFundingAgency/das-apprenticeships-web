using SFA.DAS.Apprenticeships.Domain.Api;

namespace SFA.DAS.Apprenticeships.Domain.Interfaces
{
    public interface IApiClient
    {
        Task<ApiResponse<TResponse>> Get<TResponse>(IGetApiRequest request);
        Task<ApiResponse<TResponse>> Post<TResponse>(IPostApiRequest request);
        Task<ApiResponse<TResponse>> Put<TResponse>(IPutApiRequest request);
        Task<ApiResponse<TResponse>> Delete<TResponse>(IDeleteApiRequest request);
        Task<ApiResponse<TResponse>> Patch<TResponse>(IPatchApiRequest request);
    }
}