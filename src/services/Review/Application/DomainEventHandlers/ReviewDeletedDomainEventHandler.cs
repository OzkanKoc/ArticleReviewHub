using Application.Common.Caching;
using Domain.Constants;
using Domain.Events;
using Domain.Extensions;
using MediatR;

namespace Application.DomainEventHandlers;

internal sealed class ReviewDeletedDomainEventHandler(IRedisResilienceCacheProvider cacheProvider) : INotificationHandler<ReviewDeletedDomainEvent>
{
    public async Task Handle(ReviewDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        await cacheProvider.RemoveAsync(CacheKeyConstants.Reviews, cancellationToken);
        await cacheProvider.RemoveAsync(CacheKeyConstants.Review.Format(notification.Review.Id), cancellationToken);
    }
}
