using FluentValidation;

namespace Application.Commands.Reviews.Update;

public class UpdateReviewCommandValidator : AbstractValidator<UpdateReviewCommand>
{
    public UpdateReviewCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Reviewer).NotEmpty().MaximumLength(300);
        RuleFor(x => x.ArticleId).GreaterThan(0);
        RuleFor(x => x.ReviewerContent).NotEmpty();
    }
}
