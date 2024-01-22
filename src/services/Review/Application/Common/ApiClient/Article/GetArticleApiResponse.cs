namespace Application.Common.ApiClient.Article;

public sealed record GetArticleApiResponse(
    int Id,
    string Title,
    string Author,
    string ArticleContent,
    DateTime PublishDate,
    int StarCount);
