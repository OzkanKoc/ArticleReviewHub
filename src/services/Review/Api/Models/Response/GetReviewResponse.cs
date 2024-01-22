namespace Api.Models.Response;

public sealed record GetReviewResponse(
    int Id,
    int ArticleId,
    string Reviewer,
    string ReviewerContent);
