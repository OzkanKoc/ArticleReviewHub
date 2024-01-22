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
using Domain.Options;
using Swashbuckle.AspNetCore.SwaggerUI;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

services.AddHealthChecks();

services.Configure<TokenOptions>(configuration.GetSection(TokenOptions.SectionName));

services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var jwtSettings = configuration.GetSection("Token").Get<TokenOptions>();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings!.Issuer,
            ValidateAudience = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        jwtSettings.SecretKey))
        };
    });

services.AddAuthorization();

// if (!env.IsProduction())
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "Article API", Version = "v1", Description = "Managing Article"
        });
    c.AddSecurityDefinition("Bearer",
        new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Name = "Authorization",
            Scheme = "Bearer",
            Description =
                "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
        });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme, Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
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
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Article API V1");
            c.RoutePrefix = string.Empty;
            c.DocExpansion(DocExpansion.None);
        });
    }

    app.UseAuthentication();
    app.UseAuthorization();

    using var scope = app.Services.CreateScope();
    var initializer = scope.ServiceProvider.GetRequiredService<ArticleDbContextInitializer>();
    await initializer.Initialize();
    await initializer.Seed();

    app.MapControllers();
    app.MapHealthChecks("/health");

    app.Run();
}
