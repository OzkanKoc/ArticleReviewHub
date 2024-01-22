using Domain;
using Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace Application.PipelineBehaviors;

/// <summary>
///     Validation Pipeline Behavior for MediatR
/// </summary>
/// <typeparam name="TRequest">Request Entity</typeparam>
/// <typeparam name="TResponse">Response Entity</typeparam>
public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var validationResults =
            await Task.WhenAll(validators.Select(v
                => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults.SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count == 0)
            return await next();

        var groupValidationExceptions = failures.GroupBy(x => x.PropertyName).ToList();

        var validationFailures = groupValidationExceptions.Select(x =>
                new ErrorResultDetail
                {
                    Field = x.Key, Message = x.Select(y => y.ErrorMessage)
                })
            .ToList();

        throw new CustomException(ErrorType.Validation, "validation.error", validationFailures);
    }
}
