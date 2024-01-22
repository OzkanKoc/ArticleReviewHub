using Application.Common.Repositories;
using Domain.Entities;
using Domain.Events;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands.Reviews.Delete;

public sealed record DeleteReviewCommand(int Id) : IRequest;

internal sealed class DeleteReviewCommandHandler(IRepository<Review> repository) : IRequestHandler<DeleteReviewCommand>
{
    public async Task Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await repository.Table.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (review is null)
        {
            throw new CustomException(ErrorType.NotFound);
        }

        review.AddDomainEvent(new ReviewDeletedDomainEvent(review));

        repository.Remove(review);
        await repository.SaveAll(cancellationToken);
    }
}
