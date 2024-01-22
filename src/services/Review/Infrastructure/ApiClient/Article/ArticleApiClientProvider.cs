using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.Common.ApiClient.Article;
using Application.Common.Serializer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.ApiClient.Article;

public class ArticleApiClientProvider(
    IHttpClientFactory httpClientFactory,
    IOptions<ArticleApiDefinitionOptions> options,
    ISerializer serializer,
    ILogger<ArticleApiClient> logger) : IArticleApiClientProvider
{
    public async Task<IArticleApiClient> GetApiClient(CancellationToken cancellationToken)
    {
        var clientOptions = options.Value;
        var client = httpClientFactory.CreateClient();
        client.BaseAddress = new Uri($"{clientOptions.Host}/api/v1/");
        var result = await client.PostAsJsonAsync("auth", new AuthRequest(clientOptions.ApiKey, clientOptions.ApiSecret), cancellationToken);
        if (!result.IsSuccessStatusCode)
        {
            return null;
        }

        var authResponse = await result.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponse.Token);
        return new ArticleApiClient(client, serializer, logger);
    }
}
