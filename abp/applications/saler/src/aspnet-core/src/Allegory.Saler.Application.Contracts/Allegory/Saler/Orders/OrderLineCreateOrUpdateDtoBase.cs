using Allegory.Saler.Calculations.Product;
using System;
using System.ComponentModel.DataAnnotations;

namespace Allegory.Saler.Orders;

public abstract class OrderLineCreateOrUpdateDtoBase : CalculableProductInputDto
{
    [Required]
    [EnumDataType(typeof(OrderLineType))]
    public OrderLineType Type { get; set; }

    [Required]
    public string ProductCode { get; set; }

    [Required]
    public string UnitCode { get; set; }
    
    public DateTime? ReserveDate { get; set; }

    public decimal? ReserveQuantity { get; set; }
}
