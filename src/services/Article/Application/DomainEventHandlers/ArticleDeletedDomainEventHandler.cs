using Application.Common.Caching;
using Domain.Constants;
using Domain.Events;
using Domain.Extensions;
using MediatR;

namespace Application.DomainEventHandlers;

internal sealed class ArticleDeletedDomainEventHandler(IRedisResilienceCacheProvider cacheProvider) : INotificationHandler<ArticleDeletedDomainEvent>
{
    public async Task Handle(ArticleDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        await cacheProvider.RemoveAsync(CacheKeyConstants.Articles, cancellationToken);
        await cacheProvider.RemoveAsync(CacheKeyConstants.Article.Format(notification.Article.Id), cancellationToken);
    }
}
