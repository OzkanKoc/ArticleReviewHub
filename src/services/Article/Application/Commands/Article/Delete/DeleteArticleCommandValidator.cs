using FluentValidation;

namespace Application.Commands.Article.Delete;

public class DeleteArticleCommandValidator : AbstractValidator<DeleteArticleCommand>
{
    public DeleteArticleCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
