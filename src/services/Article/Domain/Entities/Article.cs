namespace Domain.Entities;

public class Article : BaseEntity
{
    public string Title { get; set; }
    public string Author { get; set; }
    public string ArticleContent { get; set; }
    public DateTime PublishDate { get; set; }
    public int StarCount { get; set; }
}
