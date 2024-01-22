namespace Api.Models.Article.Request;

public sealed record CreateArticleRequest(
    string Title,
    string Author,
    string ArticleContent,
    DateTime PublishDate,
    int StarCount);
