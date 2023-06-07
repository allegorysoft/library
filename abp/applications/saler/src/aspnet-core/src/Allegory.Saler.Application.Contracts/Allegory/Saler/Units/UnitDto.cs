using Volo.Abp.Application.Dtos;

namespace Allegory.Saler.Units;

public class UnitDto : EntityDto<int>
{
    public string Code { get; set; }
    public string Name { get; set; }
    public bool MainUnit { get; set; }
    public decimal ConvFact1 { get; set; }
    public decimal ConvFact2 { get; set; }
    public bool Divisible { get; set; }
    public string GlobalUnitCode { get; set; }
}
