using FluentValidation;

namespace Application.Queries.Reviews.GetById;

public class GetReviewByIdQueryValidator : AbstractValidator<GetReviewByIdQuery>
{
    public GetReviewByIdQueryValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
