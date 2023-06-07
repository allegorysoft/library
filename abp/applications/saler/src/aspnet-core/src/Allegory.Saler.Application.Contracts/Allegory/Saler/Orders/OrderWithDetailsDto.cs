using Allegory.Saler.Calculations.Product;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Allegory.Saler.Orders;

public class OrderWithDetailsDto : ExtensibleAuditedEntityDto<int>, ICalculableProductAggregateRootDto<OrderLineDto>
{
    public string Number { get; set; }

    public OrderType Type { get; set; }

    public DateTime Date { get; set; }

    public OrderStatu Statu { get; set; }

    public string ClientCode { get; set; }

    public decimal TotalDiscount { get; set; }

    public decimal TotalVatBase { get; set; }

    public decimal TotalVatAmount { get; set; }

    public decimal TotalGross { get; set; }

    public string CurrencyCode { get; set; }

    public decimal? CurrencyRate { get; set; }

    public decimal? CurrencyTotalDiscount => CurrencyRate.HasValue
        ? Math.Round(TotalDiscount / CurrencyRate.Value, 2)
        : null;

    public decimal? CurrencyTotalVatBase => CurrencyRate.HasValue
        ? Math.Round(TotalVatBase / CurrencyRate.Value, 2)
        : null;

    public decimal? CurrencyTotalVatAmount => CurrencyRate.HasValue
        ? Math.Round(TotalVatAmount / CurrencyRate.Value, 2)
        : null;

    public decimal? CurrencyTotalGross => CurrencyRate.HasValue
        ? Math.Round(TotalGross / CurrencyRate.Value, 2)
        : null;

    public IList<OrderLineDto> Lines { get; set; }

    public IList<DiscountDto> Discounts { get; set; }
}
