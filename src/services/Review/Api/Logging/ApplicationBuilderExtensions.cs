namespace Api.Logging;

public static class ApplicationBuilderExtensions
{
    public static void ConfigureSerilogLogger(this IApplicationBuilder app)
    {
        SerilogBootstrapLogger.HttpEnricher.SetHttpContextAccessor(
            app.ApplicationServices.GetService(
                typeof(IHttpContextAccessor)) as IHttpContextAccessor);
    }
}