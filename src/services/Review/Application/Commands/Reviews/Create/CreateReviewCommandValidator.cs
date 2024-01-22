using FluentValidation;

namespace Application.Commands.Reviews.Create;

public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator()
    {
        RuleFor(x => x.ArticleId).GreaterThan(0);
        RuleFor(x => x.Reviewer).NotEmpty().MaximumLength(300);
        RuleFor(x => x.ReviewerContent).NotEmpty();
    }
}
