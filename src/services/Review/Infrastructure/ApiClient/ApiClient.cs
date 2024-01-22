using System.Net.Http.Json;
using Application.Common.ApiClient;
using Application.Common.Serializer;
using Microsoft.Extensions.Logging;

namespace Infrastructure.ApiClient;

public abstract class ApiClient<T>(HttpClient httpClient, ISerializer serializer, ILogger<T> logger)
{
    protected HttpClient HttpClient { get; } = httpClient;

    protected virtual async Task<ApiClientResponse<TResponseModel>> GetAsync<TResponseModel>(string requestUri)
    {
        var response = await HttpClient.GetAsync(requestUri);
        return await HandleResponseMessage<TResponseModel>(response);
    }

    protected virtual async Task<ApiClientResponse<TResponseModel>> PostAsync<TRequestModel, TResponseModel>(string uri, TRequestModel value)
    {
        var response = await HttpClient.PostAsJsonAsync(uri, value, serializer.SerializerOptions);
        return await HandleResponseMessage<TResponseModel>(response);
    }

    protected virtual async Task<ApiClientResponse> PostAsync<TRequestModel>(string uri, TRequestModel value)
    {
        var response = await HttpClient.PostAsJsonAsync(uri, value, serializer.SerializerOptions);
        return await HandleResponseMessage(response);
    }

    protected virtual async Task<ApiClientResponse<TResponseModel>> PostAsync<TResponseModel>(string uri)
    {
        var response = await HttpClient.PostAsJsonAsync(uri, serializer.SerializerOptions);
        return await HandleResponseMessage<TResponseModel>(response);
    }

    protected virtual async Task<ApiClientResponse> PostAsync(string uri)
    {
        var response = await HttpClient.PostAsync(uri, new StringContent(string.Empty));
        return await HandleResponseMessage(response);
    }

    protected virtual async Task<ApiClientResponse<TResponseModel>> PutAsync<TRequestModel, TResponseModel>(string requestUri, TRequestModel value)
    {
        var response = await HttpClient.PutAsJsonAsync(requestUri, value, serializer.SerializerOptions);
        return await HandleResponseMessage<TResponseModel>(response);
    }

    protected virtual async Task<ApiClientResponse<TResponseModel>> PutAsync<TResponseModel>(string requestUri)
    {
        var response = await HttpClient.PutAsJsonAsync(requestUri, serializer.SerializerOptions);
        return await HandleResponseMessage<TResponseModel>(response);
    }

    protected virtual async Task<ApiClientResponse> PutAsync<TRequestModel>(string requestUri, TRequestModel value)
    {
        var response = await HttpClient.PutAsJsonAsync(requestUri, value, serializer.SerializerOptions);
        return await HandleResponseMessage(response);
    }

    protected virtual async Task<ApiClientResponse> PutAsync(string requestUri)
    {
        var response = await HttpClient.PutAsJsonAsync(requestUri, new StringContent(string.Empty), serializer.SerializerOptions);
        return await HandleResponseMessage(response);
    }

    protected virtual async Task<ApiClientResponse<TResponseModel>> PatchAsync<TRequestModel, TResponseModel>(string requestUri, TRequestModel value)
    {
        var response = await HttpClient.PatchAsJsonAsync(requestUri, value, serializer.SerializerOptions);
        return await HandleResponseMessage<TResponseModel>(response);
    }

    protected virtual async Task<ApiClientResponse> PatchAsync<TRequestModel>(string requestUri, TRequestModel value)
    {
        var response = await HttpClient.PatchAsJsonAsync(requestUri, value, serializer.SerializerOptions);
        return await HandleResponseMessage(response);
    }

    protected virtual async Task<ApiClientResponse> PatchAsync(string requestUri)
    {
        var response = await HttpClient.PatchAsJsonAsync(requestUri, new StringContent(string.Empty), serializer.SerializerOptions);
        return await HandleResponseMessage(response);
    }

    protected virtual async Task<ApiClientResponse> DeleteAsync(string requestUri)
    {
        var response = await HttpClient.DeleteAsync(requestUri);
        return await HandleResponseMessage(response);
    }

    protected virtual async Task<ApiClientResponse> DeleteAsync<TResponseModel>(string requestUri)
    {
        var response = await HttpClient.DeleteAsync(requestUri);
        return await HandleResponseMessage<TResponseModel>(response);
    }

    protected virtual async Task<ApiClientResponse<T>> HandleResponseMessage<T>(HttpResponseMessage httpResponseMessage) => new ApiClientResponse<T>
    {
        Content = httpResponseMessage.IsSuccessStatusCode ? await TryReadJsonAsync<T>(httpResponseMessage) : default,
        ValidationErrors = httpResponseMessage.StatusCode == System.Net.HttpStatusCode.BadRequest ? (await TryReadJsonAsync<ValidationErrors>(httpResponseMessage))?.Errors : null,
        HttpResponseMessage = httpResponseMessage
    };

    protected virtual async Task<ApiClientResponse> HandleResponseMessage(HttpResponseMessage httpResponseMessage) => new ApiClientResponse
    {
        ValidationErrors = httpResponseMessage.StatusCode == System.Net.HttpStatusCode.BadRequest ? (await TryReadJsonAsync<ValidationErrors>(httpResponseMessage))?.Errors : null, HttpResponseMessage = httpResponseMessage
    };

    private async Task<T> TryReadJsonAsync<T>(HttpResponseMessage httpResponseMessage)
    {
        try
        {
            return await httpResponseMessage.Content.ReadFromJsonAsync<T>(serializer.SerializerOptions);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Exception occured while parsing JSON for request to '{absoluteUri}'", httpResponseMessage?.RequestMessage?.RequestUri?.AbsoluteUri);
            return default;
        }
    }

    private class ValidationErrors
    {
        public Dictionary<string, string[]> Errors { get; set; }
    }
}
