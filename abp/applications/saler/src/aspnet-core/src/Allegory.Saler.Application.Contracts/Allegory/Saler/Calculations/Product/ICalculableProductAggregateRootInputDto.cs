using System.Collections.Generic;

namespace Allegory.Saler.Calculations.Product;

public interface ICalculableProductAggregateRootInputDto<C> where C : CalculableProductInputDto
{
    string CurrencyCode { get; set; }
    decimal? CurrencyRate { get; set; }
    IList<C> Lines { get; set; }
    IList<DiscountDto> Discounts { get; set; }
}
