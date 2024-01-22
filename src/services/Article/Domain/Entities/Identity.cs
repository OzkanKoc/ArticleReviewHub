namespace Domain.Entities;

public class Identity : BaseEntity
{
    public string ApiKey { get; set; }
    public string ApiSecret { get; set; }
}
