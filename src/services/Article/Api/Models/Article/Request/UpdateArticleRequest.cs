namespace Api.Models.Article.Request;

public sealed record UpdateArticleRequest(
    string Title,
    string Author,
    string ArticleContent,
    DateTime PublishDate,
    int StarCount);
