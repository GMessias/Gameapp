namespace Gameapp.Domain.Entities;

public class Item
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set;}
    public DateTime UpdatedDate { get; set;}
}
