using FluentValidation;

namespace Application.Queries.Articles.GetById;

public class GetArticleByIdQueryValidator : AbstractValidator<GetArticleByIdQuery>
{
    public GetArticleByIdQueryValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
