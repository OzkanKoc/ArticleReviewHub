namespace Api.Models.Request;

public sealed record CreateReviewRequest(
    int ArticleId,
    string Reviewer,
    string ReviewerContent);
