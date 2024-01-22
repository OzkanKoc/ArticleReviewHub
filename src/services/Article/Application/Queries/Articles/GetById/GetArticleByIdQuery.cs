using Application.Common.Caching;
using Application.Common.Repositories;
using Application.Queries.Articles.Dto;
using Domain;
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
                return await repository.Table
                           .Where(x => x.Id == request.Id)
                           .Select(x => new GetArticleDto(
                               x.Id,
                               x.Title,
                               x.Author,
                               x.ArticleContent,
                               x.PublishDate,
                               x.StarCount))
                           .FirstOrDefaultAsync(cancellationToken)
                       ?? throw new CustomException(ErrorType.NotFound);
            }, cancellationToken);
    }
}
