using FluentValidation;

namespace Application.Commands.Article.Update;

public class UpdateArticleCommandValidator : AbstractValidator<UpdateArticleCommand>
{
    public UpdateArticleCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Author).NotEmpty().MaximumLength(300);
        RuleFor(x => x.ArticleContent).NotEmpty();
        RuleFor(x => x.PublishDate).GreaterThan(DateTime.Now.Date);
        RuleFor(x => x.StarCount).GreaterThan(0);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(300);
    }
}
