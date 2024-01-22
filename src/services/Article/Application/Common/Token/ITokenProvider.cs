namespace Application.Common.Token;

public interface ITokenProvider
{
    string GenerateToken(string apiKey, int id);
}
