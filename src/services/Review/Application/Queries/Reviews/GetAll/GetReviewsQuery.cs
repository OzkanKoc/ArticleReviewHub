using Application.Common.Caching;
using Application.Common.OData;
using Application.Common.Repositories;
using Application.Extensions;
using Application.Queries.Reviews.Dto;
using Domain.Constants;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries.Reviews.GetAll;

public sealed record GetReviewsQuery : IRequest<GetReviewsDto>;

internal sealed class GetReviewsQueryHandler(IRepository<Review> repository, IRedisResilienceCacheProvider cacheProvider, IODataQueryProvider oDataQueryProvider) : IRequestHandler<GetReviewsQuery, GetReviewsDto>
{
    public async Task<GetReviewsDto> Handle(GetReviewsQuery request, CancellationToken cancellationToken)
    {
        return await cacheProvider.GetAsync(CacheKeyConstants.Reviews,
            CacheTime.FifteenMinutes,
            async () =>
            {
                var reviews = await repository.Table
                    .ApplyODataQuery(oDataQueryProvider.ODataQuery)
                    .Select(x => new GetReviewDto(
                        x.Id,
                        x.ArticleId,
                        x.Reviewer,
                        x.ReviewContent))
                    .ToListAsync(cancellationToken);

                return new GetReviewsDto(reviews);
            }, cancellationToken);
    }
}
