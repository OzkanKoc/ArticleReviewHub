namespace Domain.Entities;

public class Review : BaseEntity
{
    public int ArticleId { get; set; }
    public string Reviewer { get; set; }
    public string ReviewContent { get; set; }
}
