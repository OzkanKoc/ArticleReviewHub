using Serilog;

namespace Api.Logging;

internal class SerilogBootstrapLogger : IBootstrapLogger
{
    public static readonly HttpEnricher HttpEnricher = new();

    public void LogInformation(string message)
    {
        Log.Information(message);
    }

    public void LogError(Exception ex, string message)
    {
        Log.Error(ex, message);
    }

#pragma warning disable CA1816
    public void Dispose()
#pragma warning restore CA1816
    {
        Log.CloseAndFlush();
    }

    public void Configure()
    {
        var configuration = new LoggerConfiguration().Enrich.With(HttpEnricher);

        // if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == Environments.Production)
        //     log somewhere

        configuration
            .MinimumLevel.Information()
            .WriteTo.Console();

        Log.Logger = configuration.CreateLogger();
    }
}
