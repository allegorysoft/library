using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Validation;

namespace Allegory.Saler.Units;

public abstract class UnitCreateOrUpdateDtoBase : EntityDto
{
    [Required]
    [DynamicStringLength(typeof(UnitConsts), nameof(UnitConsts.MaxCodeLength))]
    public string Code { get; set; }

    [DynamicStringLength(typeof(UnitConsts), nameof(UnitConsts.MaxNameLength))]
    public string Name { get; set; }

    [Required]
    public bool MainUnit { get; set; }

    [Required]
    public decimal ConvFact1 { get; set; }

    [Required]
    public decimal ConvFact2 { get; set; }

    [Required]
    public bool Divisible { get; set; }

    [DynamicStringLength(typeof(UnitConsts), nameof(UnitConsts.MaxGlobalUnitCodeLength))]
    public string GlobalUnitCode { get; set; }
}
