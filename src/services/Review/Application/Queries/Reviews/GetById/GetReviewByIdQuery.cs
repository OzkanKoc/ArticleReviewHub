using Application.Common.Caching;
using Application.Common.Repositories;
using Application.Queries.Reviews.Dto;
using Domain.Constants;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries.Reviews.GetById;

public sealed record GetReviewByIdQuery(int Id) : IRequest<GetReviewDto>;

internal sealed class GetReviewByIdQueryHandler(IRepository<Review> repository, IRedisResilienceCacheProvider cacheProvider) : IRequestHandler<GetReviewByIdQuery, GetReviewDto>
{
    public async Task<GetReviewDto> Handle(GetReviewByIdQuery request, CancellationToken cancellationToken)
    {
        return await cacheProvider.GetAsync(CacheKeyConstants.Review.Format(request.Id),
            CacheTime.FifteenMinutes,
            async () =>
            {
                return await repository.Table
                           .Where(x => x.Id == request.Id)
                           .Select(x => new GetReviewDto(
                               x.Id,
                               x.ArticleId,
                               x.Reviewer,
                               x.ReviewContent))
                           .FirstOrDefaultAsync(cancellationToken)
                       ?? throw new CustomException(ErrorType.NotFound);
            }, cancellationToken);
    }
}
