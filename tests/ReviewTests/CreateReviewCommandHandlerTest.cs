using Application.Commands.Reviews.Create;
using Application.Common.ApiClient.Article;
using Application.Common.Repositories;
using Domain.Entities;
using Domain.Exceptions;
using FluentAssertions;
using Moq;

namespace ReviewTests;

public class CreateReviewCommandHandlerTest
{
    private readonly Mock<IRepository<Review>> _repositoryMock = new();
    
    [Fact]
    public async Task ShouldThrowCustomException_When_ArticleDoesNotExist()
    {
        // Arrange
        var articleApiClientProviderMock = new Mock<IArticleApiClientProvider>();
        var articleApiClientMock = new Mock<IArticleApiClient>();
        var request = new CreateReviewCommand
        (
            1,
            "John Doe",
            "Great article!"
        );

        var handler = new CreateReviewCommandHandler(_repositoryMock.Object, articleApiClientProviderMock.Object);
        articleApiClientProviderMock.Setup(x => x.GetApiClient(It.IsAny<CancellationToken>())).ReturnsAsync(articleApiClientMock.Object);
        articleApiClientMock.Setup(x => x.GetArticleById(It.IsAny<int>())).ReturnsAsync((GetArticleApiResponse)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CustomException>(async () => await handler.Handle(request, CancellationToken.None));
        Assert.Equal(ErrorType.NotFound, exception.ErrorType);
        Assert.Equal("article.not.found", exception.Message);
    }

    [Fact]
    public async Task ShouldCreateReview_When_ArticleExist()
    {
        // Arrange
        var articleApiClientProviderMock = new Mock<IArticleApiClientProvider>();
        var articleApiClientMock = new Mock<IArticleApiClient>();
        var request = new CreateReviewCommand
        (
            1,
            "John Doe",
            "Great article!"
        );

        var handler = new CreateReviewCommandHandler(_repositoryMock.Object, articleApiClientProviderMock.Object);
        var existingArticleApiResponse = new GetArticleApiResponse
        (
            1,
            "Sample Article",
            "Author",
            "This is a sample article content.",
            DateTime.Now,
            5
        );

        articleApiClientProviderMock.Setup(x => x.GetApiClient(It.IsAny<CancellationToken>())).ReturnsAsync(articleApiClientMock.Object);
        articleApiClientMock.Setup(x => x.GetArticleById(It.IsAny<int>())).ReturnsAsync(existingArticleApiResponse);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        existingArticleApiResponse.Id.Should().Be(result.ArticleId);
        request.ArticleId.Should().Be(result.ArticleId);
        request.Reviewer.Should().Be(result.Reviewer);
        request.ReviewerContent.Should().Be(result.ReviewerContent);
    }
}
