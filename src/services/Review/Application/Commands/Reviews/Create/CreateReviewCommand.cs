using Application.Common.ApiClient.Article;
using Application.Common.Repositories;
using Application.Queries.Reviews.Dto;
using Domain.Entities;
using Domain.Events;
using Domain.Exceptions;
using MediatR;

namespace Application.Commands.Reviews.Create;

public sealed record CreateReviewCommand(
    int ArticleId,
    string Reviewer,
    string ReviewerContent) : IRequest<GetReviewDto>;

internal sealed class CreateReviewCommandHandler(IRepository<Review> repository, IArticleApiClientProvider articleApiClientProvider) : IRequestHandler<CreateReviewCommand, GetReviewDto>
{
    public async Task<GetReviewDto> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        await EnsureArticleExist(request.ArticleId, cancellationToken);
        var review = new Review
        {
            ArticleId = request.ArticleId, Reviewer = request.Reviewer, ReviewContent = request.ReviewerContent
        };

        review.AddDomainEvent(new ReviewCreatedDomainEvent(review));

        await repository.Insert(review, cancellationToken);
        await repository.SaveAll(cancellationToken);

        return new GetReviewDto(
            review.Id,
            review.ArticleId,
            review.Reviewer,
            review.ReviewContent);
    }
    private async Task EnsureArticleExist(int articleId, CancellationToken cancellationToken)
    {
        var articleApiClient = await articleApiClientProvider.GetApiClient(cancellationToken);
        var article = await articleApiClient.GetArticleById(articleId);
        if (article is null)
        {
            throw new CustomException(ErrorType.NotFound, "article.not.found");
        }
    }
}
