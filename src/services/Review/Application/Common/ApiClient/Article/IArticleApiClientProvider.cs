namespace Application.Common.ApiClient.Article;

public interface IArticleApiClientProvider
{
    Task<IArticleApiClient> GetApiClient(CancellationToken cancellationToken);
}
