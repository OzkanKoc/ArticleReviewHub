namespace Api.Models.Request;

public sealed record UpdateReviewRequest(
    int ArticleId,
    string Reviewer,
    string ReviewerContent);
