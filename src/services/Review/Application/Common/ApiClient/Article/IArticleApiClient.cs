namespace Application.Common.ApiClient.Article;

public interface IArticleApiClient
{
    Task<GetArticleApiResponse> GetArticleById(int id);
}
