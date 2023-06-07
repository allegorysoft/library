using Volo.Abp.Application.Dtos;

namespace Allegory.Saler.Services;

public class ServiceDto : ExtensibleAuditedEntityDto<int>
{
    public string MainUnitCode { get; set; }

    public string Code { get; set; }
    public string Name { get; set; }
}
