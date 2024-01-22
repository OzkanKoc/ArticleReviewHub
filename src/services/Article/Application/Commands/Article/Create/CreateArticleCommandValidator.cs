using FluentValidation;

namespace Application.Commands.Article.Create;

public class CreateArticleCommandValidator : AbstractValidator<CreateArticleCommand>
{
    public CreateArticleCommandValidator()
    {
        RuleFor(x => x.ArticleContent).NotEmpty();
        RuleFor(x => x.PublishDate).GreaterThan(DateTime.Now.Date);
        RuleFor(x => x.StarCount).GreaterThan(0);
        RuleFor(x => x.Author).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(300);
    }
}
