using Application.PipelineBehaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class Startup
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(Startup).Assembly;
        services.AddMediatR(
            x =>
            {
                x.RegisterServicesFromAssembly(assembly);
                x.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            });

        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
