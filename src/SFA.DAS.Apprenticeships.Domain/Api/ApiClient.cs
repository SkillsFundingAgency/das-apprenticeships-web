using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SFA.DAS.Apprenticeships.Domain.Interfaces;
using SFA.DAS.Apprenticeships.Infrastructure.Configuration;
using SFA.DAS.Apprenticeships.Infrastructure;

namespace SFA.DAS.Apprenticeships.Domain.Api
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ApprenticeshipsOuterApi _config;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiClient (HttpClient httpClient, IOptions<ApprenticeshipsOuterApi> config, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _config = config.Value;
            _httpClient.BaseAddress = new Uri(config.Value.BaseUrl);
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse<TResponse>> Get<TResponse>(IGetApiRequest request)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, request.GetUrl);
            AddAuthenticationHeader(requestMessage, request);
            
            var response = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);

            return await ProcessResponse<TResponse>(response);
        }
        
        public async Task<ApiResponse<TResponse>> Post<TResponse>(IPostApiRequest request)
        {
            var stringContent = request.Data != null ? new StringContent(JsonSerializer.Serialize(request.Data), Encoding.UTF8, "application/json") : null;

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, request.PostUrl);
            requestMessage.Content = stringContent;
            AddAuthenticationHeader(requestMessage, request);
            
            var response = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);

            return await ProcessResponse<TResponse>(response);
        }
        
        public async Task<ApiResponse<TResponse>> Put<TResponse>(IPutApiRequest request)
        {
            var stringContent = request.Data != null ? new StringContent(JsonSerializer.Serialize(request.Data), Encoding.UTF8, "application/json") : null;

            var requestMessage = new HttpRequestMessage(HttpMethod.Put, request.PutUrl);
            requestMessage.Content = stringContent;
            AddAuthenticationHeader(requestMessage, request);
            
            var response = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);

            return await ProcessResponse<TResponse>(response);
        }

        public async Task<ApiResponse<TResponse>> Delete<TResponse>(IDeleteApiRequest request)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, request.DeleteUrl);
            AddAuthenticationHeader(requestMessage, request);

            var response = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);

            return await ProcessResponse<TResponse>(response);
        }

        public async Task<ApiResponse<TResponse>> Patch<TResponse>(IPatchApiRequest request)
        {
            var stringContent = request.Data != null ? new StringContent(JsonSerializer.Serialize(request.Data), Encoding.UTF8, "application/json") : null;

            var requestMessage = new HttpRequestMessage(HttpMethod.Patch, request.PatchUrl);
            requestMessage.Content = stringContent;
            AddAuthenticationHeader(requestMessage, request);

            var response = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);

            return await ProcessResponse<TResponse>(response);
        }

        private void AddAuthenticationHeader(HttpRequestMessage httpRequestMessage, IApiRequest apiRequest)
        {
            httpRequestMessage.Headers.Add("Ocp-Apim-Subscription-Key", _config.Key);
            httpRequestMessage.Headers.Add("X-Version", "1");

            if (apiRequest.SendBearerToken)
            {
                var token = _httpContextAccessor.HttpContext!.GetBearerToken();
                httpRequestMessage.Headers.Add("Authorization", $"Bearer {token}");
            }

        }

        private static async Task<ApiResponse<TResponse>> ProcessResponse<TResponse>(HttpResponseMessage response)
        {
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            
            var errorContent = "";
            var responseBody = default(TResponse?);
            
            if(!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
                
                errorContent = json;
            }
            else if (!string.IsNullOrWhiteSpace(json))
            {
                responseBody = JsonSerializer.Deserialize<TResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            var apiResponse = new ApiResponse<TResponse>(responseBody, response.StatusCode, errorContent);
            
            return apiResponse;
        }
    }

    public class ApiUnauthorizedException : Exception { }
}