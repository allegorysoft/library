using Allegory.Saler.Calculations.Product;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Allegory.Saler.Orders;

public class OrderUpdateDto : OrderCreateOrUpdateDtoBase, ICalculableProductAggregateRootInputDto<OrderLineUpdateDto>
{
    [Required]
    public IList<OrderLineUpdateDto> Lines { get; set; }

    public IList<DiscountDto> Discounts { get; set; }

    public string CurrencyCode { get; set; }

    public decimal? CurrencyRate { get; set; }
}
