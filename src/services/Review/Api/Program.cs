using Api;
using Application;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Serilog;
using Api.Logging;
using Api.Middleware;
using Api.OData;
using Application.Common.OData;
using Swashbuckle.AspNetCore.SwaggerUI;
using Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;
var env = builder.Environment;

builder.Host.UseSerilog();
services.AddOptions();

builder.Services.AddControllers(options =>
{
    options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status404NotFound));
    options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest));
    options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status500InternalServerError));
    options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status409Conflict));
});

services.AddEndpointsApiExplorer();
services.AddHttpContextAccessor();
services.AddScoped<IODataQueryProvider, ODataQueryProvider>();

services.AddExceptionHandler<ExceptionHandlerMiddleware>();

services.AddApplication()
    .AddInfrastructure(configuration);

// if (!env.IsProduction())
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "Review API",
            Version = "v1",
            Description = "Managing Review"
        });
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Api.xml"));
});

await Bootstrapper.Run<SerilogBootstrapLogger>(async () => await App());
return;

async Task App()
{
    var app = builder.Build();

    app.ConfigureSerilogLogger();

    app.UseExceptionHandler(_ => { });

    // if (!env.IsProduction())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Review API V1");
            c.RoutePrefix = string.Empty;
            c.DocExpansion(DocExpansion.None);
        });
    }

    using (var scope = app.Services.CreateAsyncScope())
    {
        var initializer = scope.ServiceProvider.GetRequiredService<ReviewDbContextInitializer>();
        await initializer.Initialize();
    }

    app.MapControllers();

    app.Run();
}
