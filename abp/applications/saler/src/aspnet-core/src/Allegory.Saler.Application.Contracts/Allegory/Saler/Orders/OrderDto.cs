using System;
using Volo.Abp.Application.Dtos;

namespace Allegory.Saler.Orders;

public class OrderDto : ExtensibleAuditedEntityDto<int>
{
    public string Number { get; set; }
    public OrderType Type { get; set; }
    public OrderStatu Statu { get; set; }
    public string ClientCode { get; set; }
    public DateTime Date { get; set; }
    public decimal TotalGross { get; set; }
    public decimal? CurrencyTotalGross { get; set; }
    public string CurrencySymbol { get; set; }
}
