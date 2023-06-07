using Allegory.Saler.Calculations.Product;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Allegory.Saler.Orders;

public class OrderCreateDto : OrderCreateOrUpdateDtoBase, ICalculableProductAggregateRootInputDto<OrderLineCreateDto>
{
    [Required]
    [EnumDataType(typeof(OrderType))]
    public OrderType Type { get; set; }

    [Required]
    public IList<OrderLineCreateDto> Lines { get; set; }

    public IList<DiscountDto> Discounts { get; set; }

    public string CurrencyCode { get; set; }

    public decimal? CurrencyRate { get; set; }
}
