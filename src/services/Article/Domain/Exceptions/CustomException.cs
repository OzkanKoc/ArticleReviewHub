namespace Domain.Exceptions;

public sealed class CustomException : Exception
{
    public CustomException(ErrorType errorType) => ErrorType = errorType;

    public CustomException(ErrorType errorType, string message, params object[] args) : base(message)
    {
        ErrorType = errorType;
        MessageArgs = args;
    }

    public CustomException(
        ErrorType errorType,
        string message,
        IEnumerable<ErrorResultDetail> details) 
        : this(errorType, message, details, null)
    {
    }

    public CustomException(
        ErrorType errorType,
        string message,
        IEnumerable<ErrorResultDetail> details,
        params object[] args)
        : this(errorType, message, args)
    {
        Details = details;
    }

    public ErrorType ErrorType { get; }
    public object[] MessageArgs { get; }
    public IEnumerable<ErrorResultDetail> Details { get; }
}
