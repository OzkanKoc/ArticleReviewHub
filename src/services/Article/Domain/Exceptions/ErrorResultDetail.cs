namespace Domain.Exceptions;

public class ErrorResultDetail
{
    public string Field { get; set; }
    public IEnumerable<string> Message { get; set; }
}
