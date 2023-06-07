using Volo.Abp.Application.Dtos;

namespace Allegory.Saler.Calculations.Product;

public class DeductionDto : EntityDto
{
    public string DeductionCode { get; set; }

    public string DeductionName { get; set; }

    public short? DeductionPart1 { get; set; }

    public short? DeductionPart2 { get; set; }
}
