using System;
using System.Collections.Generic;

namespace Allegory.Saler.Calculations.Product;

public class CalculableProductAggregateRootDto : ICalculableProductAggregateRootDto<CalculableProductDto>
{
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

    public IList<CalculableProductDto> Lines { get; set; }

    public IList<DiscountDto> Discounts { get; set; }
}
