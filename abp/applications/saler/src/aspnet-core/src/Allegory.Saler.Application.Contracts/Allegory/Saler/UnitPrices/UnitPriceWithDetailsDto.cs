using System;
using Volo.Abp.Application.Dtos;

namespace Allegory.Saler.UnitPrices;

public class UnitPriceWithDetailsDto : ExtensibleAuditedEntityDto<int>
{
    public string Code { get; set; }
    public UnitPriceType Type { get; set; }
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public string UnitCode { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SalesPrice { get; set; }
    public string CurrencyCode { get; set; }
    public string ClientCode { get; set; }
    public bool IsVatIncluded { get; set; }
    public DateTime BeginDate { get; set; }
    public DateTime EndDate { get; set; }
}
