using Allegory.Saler.Calculations.Product;
using System;
using Volo.Abp.Application.Dtos;

namespace Allegory.Saler.Orders;

public class OrderLineDto : CalculableProductDto, IEntityDto<int>
{
    public int Id { get; set; }
    public OrderLineType Type { get; set; }
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public string UnitGroupCode { get; set; }
    public string UnitCode { get; set; }
    public DateTime? ReserveDate { get; set; }
    public decimal? ReserveQuantity { get; set; }
}
