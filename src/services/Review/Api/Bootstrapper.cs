using Api.Logging;

namespace Api;

internal abstract class Bootstrapper
{
    public static async Task Run<TLogger>(Func<Task> action)
        where TLogger : IBootstrapLogger, new()
    {
        var logger = new TLogger();
        logger.Configure();

        try
        {
            logger.LogInformation("Starting host...");
            await action();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Host terminated unexpectedly!");
            throw;
        }
        finally
        {
            logger.Dispose();
        }
    }
}
