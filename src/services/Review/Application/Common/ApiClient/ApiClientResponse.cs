using System.Net;

namespace Application.Common.ApiClient;

public class ApiClientResponse<TModel> : ApiClientResponse
{
    public TModel Content { get; set; }
}

public class ApiClientResponse
{
    public HttpResponseMessage HttpResponseMessage { get; set; }
    public HttpStatusCode StatusCode => HttpResponseMessage.StatusCode;
    public bool Success => HttpResponseMessage.IsSuccessStatusCode;
    public HttpResponseMessage EnsureSuccessStatusCode() => HttpResponseMessage.EnsureSuccessStatusCode();
    public Dictionary<string, string[]> ValidationErrors { get; set; }

    public async Task<string> GetContentAsync()
        => await HttpResponseMessage.Content.ReadAsStringAsync();
}
