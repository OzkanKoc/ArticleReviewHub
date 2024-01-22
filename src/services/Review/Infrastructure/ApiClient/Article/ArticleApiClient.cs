using Application.Common.ApiClient.Article;
using Application.Common.Serializer;
using Microsoft.Extensions.Logging;

namespace Infrastructure.ApiClient.Article;

public class ArticleApiClient(HttpClient httpClient, ISerializer serializer, ILogger<ArticleApiClient> logger)
    : ApiClient<ArticleApiClient>(
        httpClient,
        serializer,
        logger), IArticleApiClient
{
    public async Task<GetArticleApiResponse> GetArticleById(int id)
    {
        var apiResult = await GetAsync<GetArticleApiResponse>($"articles/{id}");
        return apiResult.Success ? apiResult.Content : null;
    }
}
