using Gameapp.Application.Enums;

namespace Gameapp.Application.Dtos;

public class ItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public EnumItemType ItemType { get; set; }
}
