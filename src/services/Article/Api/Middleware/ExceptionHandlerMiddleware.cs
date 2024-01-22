using Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace Api.Middleware;

public class ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var status = StatusCodes.Status500InternalServerError;

        string message;
        var messageArgs = Array.Empty<object>();

        IEnumerable<ErrorResultDetail> details = null;

        switch (exception)
        {
            case CustomException customException:
                status = GetStatusCode(customException.ErrorType);
                details = customException.Details;
                message = exception.Message;
                messageArgs = customException.MessageArgs;
                break;
            default:
                message = exception.ToString();
                break;
        }

        httpContext.Response.StatusCode = status;

        await LogAsync(exception, httpContext);

        await httpContext.Response.WriteAsJsonAsync(
            new RawErrorResult
            {
                Message = message, MessageArgs = messageArgs, Details = details
            }, cancellationToken);
        return true;
    }

    private static int GetStatusCode(ErrorType errorType)
        => errorType switch
        {
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

    /// <summary>
    ///     Log exception.
    /// </summary>
    /// <param name="exception">Exception.</param>
    /// <param name="context">ExceptionContext</param>
    private async Task LogAsync(Exception exception, HttpContext context)
    {
        var request = context.Request;

        var message = $"{exception.Message} {{0}} {{1}}";

        var args = new Dictionary<string, object>
        {
            {
                "RequestPath", request.Path
            },
            {
                "RequestQueryString", request.QueryString.Value ?? string.Empty
            }
        };

        var method = request.Method;
        if (method == HttpMethods.Post || method == HttpMethods.Put
                                       || method == HttpMethods.Patch)
            try
            {
                var requestBody = await GetRequestBodyAsync(context);

                args.Add("RequestBody", requestBody);

                LogByExceptionType(
                    exception,
                    context,
                    $"{message} {{2}}",
                    args.Values.ToArray());

                return;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Something went wrong while logging the request body");
            }

        LogByExceptionType(exception, context, message, args.Values.ToArray());
    }

    /// <summary>
    ///     Log exception by type.
    /// </summary>
    /// <param name="exception">Exception.</param>
    /// <param name="context">ExceptionContext</param>
    /// <param name="message">Exception message.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    private void LogByExceptionType(
        Exception exception,
        HttpContext context,
        string message,
        params object[] args)
    {
        // Log as exception, if status code is still 500.
        if (context.Response.StatusCode == StatusCodes.Status500InternalServerError)
            logger.LogError(new EventId(), exception, message, args);
        else // Otherwise, log as warning.
            logger.LogWarning(new EventId(), exception, message, args);
    }

    private static async Task<string> GetRequestBodyAsync(HttpContext context)
    {
        var stream = context.Request.Body;

        if (stream.CanSeek)
            stream.Seek(0, SeekOrigin.Begin);

        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }
}
