using Application.Common.OData;

namespace Api.OData;

public static class QueryCollectionExtensions
{
    public static ODataQuery ToODataQuery(this IQueryCollection queryCollection)
    {
        var odataQuery = new ODataQuery();

        if (queryCollection.TryGetValue("$filter", out var filter))
        {
            odataQuery.Filter = filter;
        }

        if (queryCollection.TryGetValue("$orderby", out var orderBy))
        {
            odataQuery.OrderBy = orderBy;
        }

        if (queryCollection.TryGetValue("$skip", out var skipString) && int.TryParse(skipString, out var skip))
        {
            odataQuery.Skip = skip;
        }

        if (queryCollection.TryGetValue("$take", out var takeString) && int.TryParse(takeString, out var take))
        {
            odataQuery.Take = take;
        }

        return odataQuery;
    }
}
