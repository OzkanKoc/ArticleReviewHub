using Application.Common.Caching;
using Application.Common.OData;
using Application.Common.Repositories;
using Application.Extensions;
using Application.Queries.Articles.Dto;
using Domain.Constants;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries.Articles.GetAll;

public sealed class GetArticlesQuery : IRequest<GetArticlesDto>;

internal sealed class GetArticlesQueryHandler(IRepository<Article> repository, IRedisResilienceCacheProvider cacheProvider, IODataQueryProvider oDataQueryProvider) : IRequestHandler<GetArticlesQuery, GetArticlesDto>
{
    public async Task<GetArticlesDto> Handle(GetArticlesQuery request, CancellationToken cancellationToken)
    {
        return await cacheProvider.GetAsync(CacheKeyConstants.Articles,
            CacheTime.FifteenMinutes,
            async () =>
            {
                var articles = await repository.Table
                    .ApplyODataQuery(oDataQueryProvider.ODataQuery)
                    .Select(x => new GetArticleDto(x.Id,
                        x.Title,
                        x.Author,
                        x.ArticleContent,
                        x.PublishDate,
                        x.StarCount))
                    .ToListAsync(cancellationToken);

                return new GetArticlesDto(articles);
            }, cancellationToken);
    }
}
