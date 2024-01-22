using System.Linq.Expressions;
using System.Web;
using Domain.Extensions;

namespace Application.Common.OData;

/// <summary>
/// Transforms a IQueryable to a OData query
/// Does not implement everything yet.
/// </summary>
public class ODataQuery : ODataQueryExpressionVisitor
{
    public ODataQuery() { }
    public ODataQuery(Expression expression) : base(expression) { }

    public ODataQuery WithFilter(string filter)
    {
        Filter = filter;
        return this;
    }

    public ODataQuery WithDefaultFilter(string filter)
    {
        if (Filter.IsNullOrWhiteSpace() && !filter.IsNullOrWhiteSpace())
        {
            Filter = filter;
        }

        return this;
    }

    public ODataQuery WithOrderBy(string orderBy)
    {
        OrderBy = orderBy;
        return this;
    }

    public ODataQuery WithDefaultOrderBy(string orderBy)
    {
        if (OrderBy.IsNullOrWhiteSpace() && !orderBy.IsNullOrWhiteSpace())
        {
            OrderBy = orderBy;
        }

        return this;
    }

    public ODataQuery WithSkip(int skip)
    {
        Skip = skip;
        return this;
    }

    public ODataQuery WithDefaultSkip(int? skip)
    {
        if (Skip is null && skip is not null)
        {
            Skip = skip;
        }

        return this;
    }

    public ODataQuery WithTake(int take)
    {
        Take = take;
        return this;
    }

    public ODataQuery WithDefaultTake(int? take)
    {
        if (Take is null && take is not null)
        {
            Take = take;
        }

        return this;
    }

    public static ODataQuery Translate(Expression expression) => new(expression);

    public string ToUrlParameters()
    {
        var urlParameters = string.Empty;

        if (!Filter.IsNullOrWhiteSpace())
        {
            urlParameters += $"$filter={HttpUtility.UrlEncode(Filter)}&";
        }

        if (!OrderBy.IsNullOrWhiteSpace())
        {
            urlParameters += $"$orderby={HttpUtility.UrlEncode(OrderBy)}&";
        }

        if (Skip.HasValue)
        {
            urlParameters += $"$skip={Skip}&";
        }

        if (Take.HasValue)
        {
            urlParameters += $"$take={Take}&";
        }

        return urlParameters.TrimEnd('&');
    }
}
