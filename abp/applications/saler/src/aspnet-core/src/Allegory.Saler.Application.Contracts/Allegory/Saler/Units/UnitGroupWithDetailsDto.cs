using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Allegory.Saler.Units;

public class UnitGroupWithDetailsDto : ExtensibleAuditedEntityDto<int>
{
    public string Code { get; set; }
    public string Name { get; set; }
    public IList<UnitDto> Units { get; set; }
}
