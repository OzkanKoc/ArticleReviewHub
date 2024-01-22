using System.Collections;
using System.Linq.Expressions;
using System.Text;
using Domain.Extensions;

namespace Application.Common.OData;

public class ODataQueryExpressionVisitor : ExpressionVisitor
{
    private readonly StringBuilder _stringBuilder;
    private FilterStates _filterState = FilterStates.None;

    public ODataQueryExpressionVisitor() { }

    public ODataQueryExpressionVisitor(Expression expression)
    {
        _stringBuilder = new StringBuilder();
        Visit(expression);
        Filter = _stringBuilder.ToString();
    }

    public int? Skip { get; set; }
    public int? Take { get; set; }
    public string OrderBy { get; set; }
    public string Filter { get; set; }

    [Flags]
    private enum FilterStates
    {
        None = 0,
        HasWhereQuery = 1,
        InEnumExpression = 2,
        PreviousOrderByWhenAThenBy = 4
    }

    protected override Expression VisitMethodCall(MethodCallExpression m)
    {
        System.Diagnostics.Debug.WriteLine($"VisitMethodCall: {m.Method.Name} | Type: {m.Method.DeclaringType?.Name ?? string.Empty}");

        if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Where")
        {
            Visit(m.Arguments[0]);
            var lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);

            if (_filterState.HasFlag(FilterStates.HasWhereQuery))
            {
                _stringBuilder.Append(" and ");
            }

            Visit(lambda.Body);

            _filterState |= FilterStates.HasWhereQuery;

            return m;
        }

        switch (m.Method.Name)
        {
            case "Contains":
            {
                var value = Expression.Lambda(m.Arguments[0]).Compile().DynamicInvoke();

                if (value is IEnumerable list and not string)
                {
                    _stringBuilder.Append('(');

                    var hasAddedFirstItem = false;

                    foreach (var item in list)
                    {
                        if (hasAddedFirstItem)
                        {
                            _stringBuilder.Append(" or ");
                        }

                        Visit(m.Arguments[1]);
                        _stringBuilder.Append(" eq ");
                        AppendByValueType(item);

                        hasAddedFirstItem = true;
                    }

                    _stringBuilder.Append(')');
                }
                else
                {
                    _stringBuilder.Append("contains(");
                    Visit(m.Object);
                    _stringBuilder.Append(',');
                    AppendByValueType(value);
                    _stringBuilder.Append(')');
                }

                return m;
            }
            case "StartsWith":
                _stringBuilder.Append("startswith(");
                Visit(m.Object);
                _stringBuilder.Append(',');
                Visit(m.Arguments[0]);
                _stringBuilder.Append(')');

                return m;
            case "EndsWith":
                _stringBuilder.Append("endswith(");
                Visit(m.Object);
                _stringBuilder.Append(',');
                Visit(m.Arguments[0]);
                _stringBuilder.Append(')');

                return m;
            case "Take":
            {
                if (ParseTakeExpression(m))
                {
                    var nextExpression = m.Arguments[0];
                    return Visit(nextExpression);
                }

                break;
            }
            case "Skip":
            {
                if (ParseSkipExpression(m))
                {
                    var nextExpression = m.Arguments[0];
                    return Visit(nextExpression);
                }

                break;
            }
            case "OrderBy":
            {
                if (ParseOrderByExpression(m, "ASC", false))
                {
                    var nextExpression = m.Arguments[0];
                    return Visit(nextExpression);
                }

                break;
            }
            case "ThenBy":
            {
                if (ParseOrderByExpression(m, "ASC", true))
                {
                    var nextExpression = m.Arguments[0];
                    return Visit(nextExpression);
                }

                break;
            }
            case "OrderByDescending":
            {
                if (ParseOrderByExpression(m, "DESC", false))
                {
                    var nextExpression = m.Arguments[0];
                    return Visit(nextExpression);
                }

                break;
            }
            case "ThenByDescending":
            {
                if (ParseOrderByExpression(m, "DESC", true))
                {
                    var nextExpression = m.Arguments[0];
                    return Visit(nextExpression);
                }

                break;
            }
            default:
                try
                {
                    var result = Expression.Lambda(m).Compile().DynamicInvoke();
                    AppendByValueType(result);

                    return m;
                }
                catch
                {
                    // ignored
                }

                break;
        }

        throw new NotSupportedException($"The method '{m.Method.Name}' is not supported");
    }

    private static Expression StripQuotes(Expression e)
    {
        while (e.NodeType == ExpressionType.Quote)
        {
            e = ((UnaryExpression)e).Operand;
        }

        return e;
    }

    protected override Expression VisitUnary(UnaryExpression u)
    {
        switch (u.NodeType)
        {
            case ExpressionType.Not:
                _stringBuilder.Append(" NOT ");
                Visit(u.Operand);
                break;
            case ExpressionType.Convert:
                Visit(u.Operand);
                break;
            default:
                throw new NotSupportedException($"The unary operator '{u.NodeType}' is not supported");
        }

        return u;
    }

    protected override Expression VisitBinary(BinaryExpression b)
    {
        _stringBuilder.Append('(');

        Visit(b.Left);

        switch (b.NodeType)
        {
            case ExpressionType.And:
                _stringBuilder.Append(" and ");
                break;

            case ExpressionType.AndAlso:
                _stringBuilder.Append(" and ");
                break;

            case ExpressionType.Or:
                _stringBuilder.Append(" or ");
                break;

            case ExpressionType.OrElse:
                _stringBuilder.Append(" or ");
                break;

            case ExpressionType.Equal:
                _stringBuilder.Append(" eq ");
                break;

            case ExpressionType.NotEqual:
                _stringBuilder.Append(" ne ");
                break;

            case ExpressionType.LessThan:
                _stringBuilder.Append(" lt ");
                break;

            case ExpressionType.LessThanOrEqual:
                _stringBuilder.Append(" le ");
                break;

            case ExpressionType.GreaterThan:
                _stringBuilder.Append(" gt ");
                break;

            case ExpressionType.GreaterThanOrEqual:
                _stringBuilder.Append(" ge ");
                break;

            default:
                throw new NotSupportedException($"The binary operator '{b.NodeType}' is not supported");
        }

        Visit(b.Right);

        _stringBuilder.Append(')');

        _filterState &= ~FilterStates.InEnumExpression;

        return b;
    }

    protected override Expression VisitConstant(ConstantExpression c)
    {
        var q = c.Value as IQueryable;

        switch (q)
        {
            case null when c.Value == null:
                _stringBuilder.Append("null");
                break;
            case null:
                AppendByValueType(c.Value);
                break;
        }

        return c;
    }

    // should only be hit for parameter names
    protected override Expression VisitMember(MemberExpression m)
    {
        if (m.Expression?.NodeType == ExpressionType.Parameter)
        {
            _stringBuilder.Append(m.Member.Name);

            if (m.Type.IsEnum)
            {
                _filterState |= FilterStates.InEnumExpression;
            }

            return m;
        }

        try
        {
            var result = Expression.Lambda(m).Compile().DynamicInvoke();
            AppendByValueType(result);

            return m;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        throw new NotSupportedException($"The member '{m.Member.Name}' is not supported. NodeType: '{m.NodeType}'");
    }

    private void AppendByValueType(object value)
    {
        switch (Type.GetTypeCode(value.GetType()))
        {
            case TypeCode.Boolean:
                _stringBuilder.Append(((bool)value) ? 1 : 0);
                break;

            case TypeCode.String:
                _stringBuilder.Append('\'');
                _stringBuilder.Append(value);
                _stringBuilder.Append('\'');
                break;

            case TypeCode.DateTime when (value is DateTime dateTime):
                _stringBuilder.Append(new DateTimeOffset(dateTime).ToString("O"));
                break;

            case TypeCode.Object when (value is Guid guid):
                _stringBuilder.Append(guid.ToString());
                break;

            case TypeCode.Object:
                throw new NotSupportedException($"The constant for '{value}' is not supported");

            case TypeCode.Int32 or TypeCode.Int64 when (_filterState.HasFlag(FilterStates.InEnumExpression)):
                _stringBuilder.Append('\'');
                _stringBuilder.Append(value);
                _stringBuilder.Append('\'');
                break;

            default:
                _stringBuilder.Append(value);
                break;
        }
    }

    private bool ParseOrderByExpression(MethodCallExpression expression, string order, bool isThenBy)
    {
        var unary = (UnaryExpression)expression.Arguments[1];

        var lambdaExpression = (LambdaExpression)unary.Operand;
        if (lambdaExpression.Body is not MemberExpression memberExpression)
        {
            return false;
        }

        if (string.IsNullOrEmpty(OrderBy))
        {
            OrderBy = $"{memberExpression.Member.Name} {order}";
        }
        else
        {
            // then expression is resolved in reseverse, so if there is already an order by present, we want the next one to occure first
            // but only if it's a preceded by a .ThenBy[Descending](...).
            // else we just ignore it since we want the last OrderBy(...)
            if (_filterState.HasFlag(FilterStates.PreviousOrderByWhenAThenBy))
            {
                OrderBy = "{1} {2}, {0}".Format(OrderBy, memberExpression.Member.Name, order);
            }
        }

        if (isThenBy)
        {
            _filterState |= FilterStates.PreviousOrderByWhenAThenBy;
        }
        else
        {
            _filterState &= ~FilterStates.PreviousOrderByWhenAThenBy;
        }

        return true;
    }

    private bool ParseTakeExpression(MethodCallExpression expression)
    {
        var sizeExpression = (ConstantExpression)expression.Arguments[1];

        if (!int.TryParse(sizeExpression.Value?.ToString(), out var size))
        {
            return false;
        }

        Take = size;
        return true;
    }

    private bool ParseSkipExpression(MethodCallExpression expression)
    {
        var sizeExpression = (ConstantExpression)expression.Arguments[1];

        if (!int.TryParse(sizeExpression.Value?.ToString(), out var size))
        {
            return false;
        }

        Skip = size;
        return true;
    }
}
