using Application.Commands.Article.Create;
using Application.Common.Repositories;
using Domain.Entities;
using FluentAssertions;
using Moq;

namespace ArticleTests;

public class CreateArticleCommandHandlerTests
{
    private readonly Mock<IRepository<Article>> _repositoryMock = new();

    [Fact]
    public async Task Handle_Should_ReturnArticle()
    {
        // Arrange
        var command = new CreateArticleCommand
        (
            "Title",
            "Author",
            "Content",
            DateTime.Now,
            3
        );

        var handler = new CreateArticleCommandHandler(_repositoryMock.Object);

        // Act
        var result = await handler.Handle(command, default);

        //Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(command.Title);
        result.Author.Should().Be(command.Author);
        result.ArticleContent.Should().Be(command.ArticleContent);
        result.PublishDate.Should().Be(command.PublishDate);
        result.StarCount.Should().Be(command.StarCount);
    }
}
