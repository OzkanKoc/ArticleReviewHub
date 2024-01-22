using Application.Common.Caching;
using Domain.Constants;
using Domain.Events;
using MediatR;

namespace Application.DomainEventHandlers;

internal sealed class ArticleCreatedDomainEventHandler(IRedisResilienceCacheProvider cacheProvider) : INotificationHandler<ArticleCreatedDomainEvent>
{
    public async Task Handle(ArticleCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await cacheProvider.RemoveAsync(CacheKeyConstants.Articles, cancellationToken);
    }
}
