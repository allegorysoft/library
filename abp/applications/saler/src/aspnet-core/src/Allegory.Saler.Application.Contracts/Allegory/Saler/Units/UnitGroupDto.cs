using Volo.Abp.Application.Dtos;

namespace Allegory.Saler.Units;

public class UnitGroupDto : ExtensibleAuditedEntityDto<int>
{
    public string Code { get; set; }
    public string Name { get; set; }
    //TODO can be added SubUnitCount public int UnitCount { get; set; }
}
