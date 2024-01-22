using Application.Common.Caching;
using Application.Common.Repositories;
using Application.Queries.Articles.Dto;
using Domain.Constants;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries.Articles.GetById;

public sealed record GetArticleByIdQuery(int Id) : IRequest<GetArticleDto>;

internal sealed class GetArticleByIdQueryHandler(IRepository<Article> repository, IRedisResilienceCacheProvider cacheProvider) : IRequestHandler<GetArticleByIdQuery, GetArticleDto>
{
    public async Task<GetArticleDto> Handle(GetArticleByIdQuery request, CancellationToken cancellationToken)
    {
        return await cacheProvider.GetAsync(CacheKeyConstants.Article.Format(request.Id),
            CacheTime.FifteenMinutes,
            async () =>
            {
                var x = await repository.Table
                            .Where(x => x.Id == request.Id)
                            .FirstOrDefaultAsync(cancellationToken)
                        ?? throw new CustomException(ErrorType.NotFound);

                return new GetArticleDto(
                    x.Id,
                    x.Title,
                    x.Author,
                    x.ArticleContent,
                    x.PublishDate,
                    x.StarCount);
            }, cancellationToken);
    }
}
