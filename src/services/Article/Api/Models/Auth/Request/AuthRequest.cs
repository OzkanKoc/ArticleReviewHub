namespace Api.Models.Auth.Request;

public sealed record AuthRequest(string ApiKey, string ApiSecret);
