using Application.Common.Caching;
using Domain.Constants;
using Domain.Events;
using Domain.Extensions;
using MediatR;

namespace Application.DomainEventHandlers;

internal sealed class ReviewUpdatedDomainEventHandler(IRedisResilienceCacheProvider cacheProvider) : INotificationHandler<ReviewUpdatedDomainEvent>
{
    public async Task Handle(ReviewUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await cacheProvider.RemoveAsync(CacheKeyConstants.Reviews, cancellationToken);
        await cacheProvider.RemoveAsync(CacheKeyConstants.Review.Format(notification.Review.Id), cancellationToken);
    }
}
