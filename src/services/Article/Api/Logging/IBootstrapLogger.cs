namespace Api.Logging;

internal interface IBootstrapLogger : IDisposable
{
    void Configure();
    void LogInformation(string message);
    void LogError(Exception ex, string message);
}