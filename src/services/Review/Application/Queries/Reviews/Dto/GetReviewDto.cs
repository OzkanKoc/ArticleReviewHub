namespace Application.Queries.Reviews.Dto;

public sealed record GetReviewDto(
    int Id,
    int ArticleId,
    string Reviewer,
    string ReviewerContent);
