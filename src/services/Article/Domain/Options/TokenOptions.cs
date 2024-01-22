namespace Domain.Options;

public class TokenOptions
{
    public const string SectionName = "Token";
    public string Issuer { get; set; }
    public string SecretKey { get; set; }
}
