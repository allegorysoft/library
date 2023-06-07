using Volo.Abp.Application.Dtos;

namespace Allegory.Saler.Items;

public class ItemDto : ExtensibleAuditedEntityDto<int>
{
    public string MainUnitCode { get; set; }

    public string Code { get; set; }
    public string Name { get; set; }
    public ItemType Type { get; set; }
    public decimal? Stock { get; set; }
    public decimal? ReservedStock { get; set; }
}
 