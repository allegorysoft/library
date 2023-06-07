using System.ComponentModel.DataAnnotations;

namespace Allegory.Saler.Items;

public class ItemCreateDto : ItemCreateOrUpdateDtoBase
{
    [Required]
    [EnumDataType(typeof(ItemType))]
    public ItemType Type { get; set; } = ItemType.Item;
}
