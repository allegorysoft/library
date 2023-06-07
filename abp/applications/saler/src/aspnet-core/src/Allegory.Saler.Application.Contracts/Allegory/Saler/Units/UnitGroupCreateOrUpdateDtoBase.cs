using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Validation;

namespace Allegory.Saler.Units;

public abstract class UnitGroupCreateOrUpdateDtoBase : ExtensibleEntityDto
{
    [Required]
    [DynamicStringLength(typeof(UnitGroupConsts), nameof(UnitGroupConsts.MaxCodeLength))]
    public string Code { get; set; }

    [DynamicStringLength(typeof(UnitGroupConsts), nameof(UnitGroupConsts.MaxNameLength))]
    public string Name { get; set; }
}
