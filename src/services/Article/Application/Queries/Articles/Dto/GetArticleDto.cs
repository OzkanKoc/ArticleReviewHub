namespace Application.Queries.Articles.Dto;

public sealed record GetArticleDto(
    int Id,
    string Title,
    string Author,
    string ArticleContent,
    DateTime PublishDate,
    int StarCount);
