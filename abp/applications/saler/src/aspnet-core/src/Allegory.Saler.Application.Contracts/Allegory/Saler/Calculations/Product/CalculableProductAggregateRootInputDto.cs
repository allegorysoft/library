using System.Collections.Generic;

namespace Allegory.Saler.Calculations.Product;

public class CalculableProductAggregateRootInputDto : ICalculableProductAggregateRootInputDto<CalculableProductInputDto>
{
    public string CurrencyCode { get; set; }
    public decimal? CurrencyRate { get; set; }
    public IList<CalculableProductInputDto> Lines { get; set; }
    public IList<DiscountDto> Discounts { get; set; }
}