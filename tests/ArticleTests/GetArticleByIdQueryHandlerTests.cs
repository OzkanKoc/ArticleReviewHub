using Application.Common.Caching;
using Application.Common.Repositories;
using Application.Queries.Articles.GetById;
using Domain.Entities;
using Domain.Exceptions;
using Moq;

namespace ArticleTests;

public class GetArticleByIdQueryHandlerTests
{
    private readonly Mock<IRepository<Article>> _repositoryMock = new();
    
    [Fact]
    public void Handle_Should_ThrowCustomException_WhenNotExist()
    {
        // Arrange
        var query = new GetArticleByIdQuery(1);
        var cacheProviderMock = new Mock<IRedisResilienceCacheProvider>();

        var handler = new GetArticleByIdQueryHandler(_repositoryMock.Object, cacheProviderMock.Object);

        // Act & Assert
        Assert.ThrowsAsync<CustomException>(async () => await handler.Handle(query, CancellationToken.None));
    }
}
