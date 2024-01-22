namespace Domain.Exceptions;

public class ErrorResult
{
    /// <summary>
    ///     Error message to notify client.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    ///     Error Details
    /// </summary>
    public IEnumerable<ErrorResultDetail> Details { get; set; }
}

public class RawErrorResult : ErrorResult
{
    public object[] MessageArgs { get; set; }
}
