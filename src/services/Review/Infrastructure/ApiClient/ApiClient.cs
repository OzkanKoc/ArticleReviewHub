using System.Net.Http.Json;
using Application.Common.ApiClient;
using Application.Common.Serializer;
using Microsoft.Extensions.Logging;

namespace Infrastructure.ApiClient;

public abstract class ApiClient<TClient>(HttpClient httpClient, ISerializer serializer, ILogger<TClient> logger)
{
    private HttpClient HttpClient { get; } = httpClient;

    protected async Task<ApiClientResponse<TResponseModel>> GetAsync<TResponseModel>(string requestUri)
    {
        var response = await HttpClient.GetAsync(requestUri);
        return await HandleResponseMessage<TResponseModel>(response);
    }

    protected async Task<ApiClientResponse<TResponseModel>> PostAsync<TRequestModel, TResponseModel>(string uri, TRequestModel value)
    {
        var response = await HttpClient.PostAsJsonAsync(uri, value, serializer.SerializerOptions);
        return await HandleResponseMessage<TResponseModel>(response);
    }

    protected async Task<ApiClientResponse> PostAsync<TRequestModel>(string uri, TRequestModel value)
    {
        var response = await HttpClient.PostAsJsonAsync(uri, value, serializer.SerializerOptions);
        return HandleResponseMessage(response);
    }

    protected async Task<ApiClientResponse<TResponseModel>> PostAsync<TResponseModel>(string uri)
    {
        var response = await HttpClient.PostAsJsonAsync(uri, serializer.SerializerOptions);
        return await HandleResponseMessage<TResponseModel>(response);
    }

    protected async Task<ApiClientResponse> PostAsync(string uri)
    {
        var response = await HttpClient.PostAsync(uri, new StringContent(string.Empty));
        return HandleResponseMessage(response);
    }

    protected async Task<ApiClientResponse<TResponseModel>> PutAsync<TRequestModel, TResponseModel>(string requestUri, TRequestModel value)
    {
        var response = await HttpClient.PutAsJsonAsync(requestUri, value, serializer.SerializerOptions);
        return await HandleResponseMessage<TResponseModel>(response);
    }

    protected async Task<ApiClientResponse<TResponseModel>> PutAsync<TResponseModel>(string requestUri)
    {
        var response = await HttpClient.PutAsJsonAsync(requestUri, serializer.SerializerOptions);
        return await HandleResponseMessage<TResponseModel>(response);
    }

    protected async Task<ApiClientResponse> PutAsync<TRequestModel>(string requestUri, TRequestModel value)
    {
        var response = await HttpClient.PutAsJsonAsync(requestUri, value, serializer.SerializerOptions);
        return HandleResponseMessage(response);
    }

    protected async Task<ApiClientResponse> PutAsync(string requestUri)
    {
        var response = await HttpClient.PutAsJsonAsync(requestUri, new StringContent(string.Empty), serializer.SerializerOptions);
        return HandleResponseMessage(response);
    }

    protected async Task<ApiClientResponse<TResponseModel>> PatchAsync<TRequestModel, TResponseModel>(string requestUri, TRequestModel value)
    {
        var response = await HttpClient.PatchAsJsonAsync(requestUri, value, serializer.SerializerOptions);
        return await HandleResponseMessage<TResponseModel>(response);
    }

    protected async Task<ApiClientResponse> PatchAsync<TRequestModel>(string requestUri, TRequestModel value)
    {
        var response = await HttpClient.PatchAsJsonAsync(requestUri, value, serializer.SerializerOptions);
        return HandleResponseMessage(response);
    }

    protected async Task<ApiClientResponse> DeleteAsync(string requestUri)
    {
        var response = await HttpClient.DeleteAsync(requestUri);
        return HandleResponseMessage(response);
    }

    private async Task<ApiClientResponse<T>> HandleResponseMessage<T>(HttpResponseMessage httpResponseMessage)
        => new()
        {
            Content = httpResponseMessage.IsSuccessStatusCode
                ? await TryReadJsonAsync<T>(httpResponseMessage)
                : default,
            HttpResponseMessage = httpResponseMessage
        };

    private static ApiClientResponse HandleResponseMessage(HttpResponseMessage httpResponseMessage)
        => new()
        {
            HttpResponseMessage = httpResponseMessage
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
}
