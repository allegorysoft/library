using Volo.Abp.Application.Dtos;

namespace Allegory.Saler.Services;

public class ServiceWithDetailsDto : ExtensibleAuditedEntityDto<int>
{
    public string UnitGroupCode { get; set; }

    public string Code { get; set; }
    public string Name { get; set; }
    public string DeductionCode { get; set; }
    public short? SalesDeductionPart1 { get; set; }
    public short? SalesDeductionPart2 { get; set; }
    public short? PurchaseDeductionPart1 { get; set; }
    public short? PurchaseDeductionPart2 { get; set; }
    public byte SalesVatRate { get; set; }
    public byte PurchaseVatRate { get; set; }
}
