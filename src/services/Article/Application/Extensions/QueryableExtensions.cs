using System.Text.RegularExpressions;
using Application.Common.OData;
using Community.OData.Linq;
using Domain.Extensions;

namespace Application.Extensions;

public static class QueryableExtensions
{
    private const string FilterPattern = @"\B'(.*?)'\B|\B'(.*[^'\s])";

    public static IQueryable<T> ApplyODataQuery<T>(this IQueryable<T> queryable, ODataQuery oDataQuery)
    {
        if (oDataQuery == null)
        {
            return queryable;
        }

        if (!oDataQuery.Filter.IsNullOrWhiteSpace())
        {
            //replaces ' char as ''(double quotes) in filter value
            var matchedStr = Regex.Match(oDataQuery.Filter, FilterPattern).ToString().Trim('\'').Replace("'", "''");
            if (matchedStr.Contains('\''))
            {
                oDataQuery.Filter = Regex.Replace(oDataQuery.Filter, FilterPattern, $"'{matchedStr}'");
            }

            queryable = queryable.OData(i => i.QuerySettings.DefaultTimeZone = TimeZoneInfo.Utc).Filter(oDataQuery.Filter).ToOriginalQuery();
        }

        if (!oDataQuery.OrderBy.IsNullOrWhiteSpace())
        {
            queryable = queryable.OData(i => i.QuerySettings.DefaultTimeZone = TimeZoneInfo.Utc).OrderBy(oDataQuery.OrderBy).ToOriginalQuery();
        }

        if (oDataQuery.Skip != null)
        {
            queryable = queryable.Skip(oDataQuery.Skip ?? 0);
        }

        if (oDataQuery.Take != null)
        {
            queryable = queryable.Take(oDataQuery.Take ?? 0);
        }

        return queryable;
    }
}
