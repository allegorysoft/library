using Volo.Abp.Application.Dtos;

namespace Allegory.Saler.Clients;

public class ClientDto : ExtensibleAuditedEntityDto<int>
{
    public ClientType Type { get; set; }
    public string Code { get; set; }
    public string Title { get; set; }
    public string IdentityNumber { get; set; }
}
