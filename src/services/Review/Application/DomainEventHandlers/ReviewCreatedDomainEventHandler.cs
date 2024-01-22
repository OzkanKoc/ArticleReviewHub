using Application.Common.Caching;
using Domain.Constants;
using Domain.Events;
using MediatR;

namespace Application.DomainEventHandlers;

internal sealed class ReviewCreatedDomainEventHandler(IRedisResilienceCacheProvider cacheProvider) : INotificationHandler<ReviewCreatedDomainEvent>
{
    public async Task Handle(ReviewCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await cacheProvider.RemoveAsync(CacheKeyConstants.Reviews, cancellationToken);
    }
}
