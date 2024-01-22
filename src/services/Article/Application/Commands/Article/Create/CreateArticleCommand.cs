using Application.Common.Repositories;
using Application.Queries.Articles.Dto;
using Domain.Events;
using MediatR;

namespace Application.Commands.Article.Create;

public sealed record CreateArticleCommand(
    string Title,
    string Author,
    string ArticleContent,
    DateTime PublishDate,
    int StarCount) : IRequest<GetArticleDto>;

internal sealed class CreateArticleCommandHandler(IRepository<Domain.Entities.Article> repository) : IRequestHandler<CreateArticleCommand, GetArticleDto>
{
    public async Task<GetArticleDto> Handle(CreateArticleCommand request, CancellationToken cancellationToken)
    {
        var article = new Domain.Entities.Article
        {
            Author = request.Author,
            ArticleContent = request.ArticleContent,
            PublishDate = request.PublishDate,
            StarCount = request.StarCount,
            Title = request.Title
        };

        article.AddDomainEvent(new ArticleCreatedDomainEvent(article));
        
        await repository.Insert(article, cancellationToken);
        await repository.SaveAll(cancellationToken);

        return new GetArticleDto(
            article.Id,
            article.Title,
            article.Author,
            article.ArticleContent,
            article.PublishDate,
            article.StarCount);
    }
}
