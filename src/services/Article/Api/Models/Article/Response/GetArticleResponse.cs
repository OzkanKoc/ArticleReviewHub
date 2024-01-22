namespace Api.Models.Article.Response;

public sealed record GetArticleResponse(
    int Id,
    string Title,
    string Author,
    string ArticleContent,
    DateTime PublishDate,
    int StarCount);
