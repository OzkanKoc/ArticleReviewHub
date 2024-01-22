using Application.Common.Repositories;
using Domain.Entities;
using Domain.Events;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands.Reviews.Update;

public sealed record UpdateReviewCommand(
    int Id,
    int ArticleId,
    string Reviewer,
    string ReviewerContent) : IRequest;

internal sealed class UpdateReviewCommandHandler(IRepository<Review> repository) : IRequestHandler<UpdateReviewCommand>
{
    public async Task Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await repository.Table.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken) ?? throw new CustomException(ErrorType.NotFound);

        review.Reviewer = request.Reviewer;
        review.ReviewContent = request.ReviewerContent;
        review.ArticleId = request.ArticleId;
        
        review.AddDomainEvent(new ReviewUpdatedDomainEvent(review));

        await repository.SaveAll(cancellationToken);
    }
}
