using Volo.Abp.Application.Dtos;

namespace Allegory.Saler.Calculations.Product;

public class DiscountDto : EntityDto
{
    public int? Id { get; set; }
    public decimal Rate { get; set; }
    public decimal Total { get; set; }
}
