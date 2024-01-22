using Application.Common.Repositories;
using Domain;
using Domain.Events;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands.Article.Delete;

public sealed record DeleteArticleCommand(int Id) : IRequest;

internal sealed class DeleteArticleCommandHandler(IRepository<Domain.Entities.Article> repository) : IRequestHandler<DeleteArticleCommand>
{
    public async Task Handle(DeleteArticleCommand request, CancellationToken cancellationToken)
    {
        var article = await repository.Table.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (article is null)
        {
            throw new CustomException(ErrorType.NotFound);
        }

        article.AddDomainEvent(new ArticleDeletedDomainEvent(article));

        repository.Remove(article);
        await repository.SaveAll(cancellationToken);
    }
}
