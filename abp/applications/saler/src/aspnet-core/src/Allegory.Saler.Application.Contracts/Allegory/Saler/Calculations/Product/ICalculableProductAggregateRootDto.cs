using System.Collections.Generic;

namespace Allegory.Saler.Calculations.Product;

public interface ICalculableProductAggregateRootDto<C>
    where C : CalculableProductDto
{
    decimal TotalDiscount { get; set; }

    decimal TotalVatBase { get; set; }

    decimal TotalVatAmount { get; set; }

    decimal TotalGross { get; set; }

    string CurrencyCode { get; set; }

    decimal? CurrencyRate { get; set; }

    decimal? CurrencyTotalDiscount { get; }

    decimal? CurrencyTotalVatBase { get; }

    decimal? CurrencyTotalVatAmount { get; }

    decimal? CurrencyTotalGross { get; }

    IList<C> Lines { get; set; }

    IList<DiscountDto> Discounts { get; set; }
}
