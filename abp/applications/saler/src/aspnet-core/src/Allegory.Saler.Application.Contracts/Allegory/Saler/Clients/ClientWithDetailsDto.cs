using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Allegory.Saler.Clients;

public class ClientWithDetailsDto : ExtensibleAuditedEntityDto<int>
{
    public ClientType Type { get; set; }
    public string Code { get; set; }
    public string Title { get; set; }
    public string IdentityNumber { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string TaxOffice { get; set; }
    public string Phone1 { get; set; }
    public string Phone2 { get; set; }
    public string Phone3 { get; set; }
    public string EMail { get; set; }
    public string KepAddress { get; set; }
}
