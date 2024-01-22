using Application.Common.Repositories;
using Domain.Events;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands.Article.Update;

public sealed record UpdateArticleCommand(
    int Id,
    string Title,
    string Author,
    string ArticleContent,
    DateTime PublishDate,
    int StarCount
) : IRequest;

internal sealed class UpdateArticleCommandHandler(IRepository<Domain.Entities.Article> repository) : IRequestHandler<UpdateArticleCommand>
{
    public async Task Handle(UpdateArticleCommand request, CancellationToken cancellationToken)
    {
        var article = await repository.Table.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken) ?? throw new CustomException(ErrorType.NotFound);

        if (article is null)
        {
            throw new CustomException(ErrorType.NotFound, "article.not.found");
        }

        article.Title = request.Title;
        article.Author = request.Author;
        article.ArticleContent = request.ArticleContent;
        article.PublishDate = request.PublishDate;
        article.StarCount = request.StarCount;

        article.AddDomainEvent(new ArticleUpdatedDomainEvent(article));

        await repository.SaveAll(cancellationToken);
    }
}
