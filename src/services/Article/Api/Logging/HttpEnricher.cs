using Serilog.Core;
using Serilog.Events;

namespace Api.Logging;

public class HttpEnricher : ILogEventEnricher
{
    private const string LogItemsKey = "LogItems";

    private IHttpContextAccessor _httpContextAccessor;

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        try
        {
            var context = _httpContextAccessor?.HttpContext;
            var request = context?.Request;
            if (request == null) return;

            if (!context.Items.TryGetValue(LogItemsKey, out var items))
            {
                items = GetLogItems(context.Request.Headers);
                context.Items.Add(LogItemsKey, items);
            }

            foreach (var item in ((Dictionary<string, string>)items)!)
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(item.Key, item.Value));
        }
        catch (Exception e)
        {
            logEvent.AddPropertyIfAbsent(
                propertyFactory.CreateProperty(nameof(HttpEnricher), $"Failed. {e.Message}"));
        }
    }

    public void SetHttpContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private static Dictionary<string, string> GetLogItems(IHeaderDictionary headers)
    {
        return headers.ToDictionary(x => x.Key, x => x.Value.ToString());
    }
}