using Application.Common.OData;

namespace Api.OData;

public class ODataQueryProvider(IHttpContextAccessor httpContextAccessor) : IODataQueryProvider
{
    public ODataQuery ODataQuery
        => httpContextAccessor?.HttpContext?.Request.Query.Count != 0
            ? httpContextAccessor?.HttpContext?.Request.Query.ToODataQuery()
            : null;
}
