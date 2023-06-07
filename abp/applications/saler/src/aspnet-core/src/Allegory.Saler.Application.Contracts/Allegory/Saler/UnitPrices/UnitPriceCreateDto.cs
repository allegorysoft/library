using System.ComponentModel.DataAnnotations;

namespace Allegory.Saler.UnitPrices;

public class UnitPriceCreateDto : UnitPriceCreateOrUpdateDtoBase
{
    [Required]
    [EnumDataType(typeof(UnitPriceType))]
    public UnitPriceType Type { get; set; } = UnitPriceType.Item;
}
