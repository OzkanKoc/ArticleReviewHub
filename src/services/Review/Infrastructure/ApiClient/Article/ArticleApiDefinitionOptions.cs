namespace Infrastructure.ApiClient.Article;

public class ArticleApiDefinitionOptions
{
    public const string SectionName = "ApiDefinitions:Article";

    public string Host { get; set; }
    public string ApiKey { get; set; }
    public string ApiSecret { get; set; }
}
