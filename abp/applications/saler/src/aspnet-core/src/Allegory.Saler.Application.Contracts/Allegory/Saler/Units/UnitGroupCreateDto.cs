using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Allegory.Saler.Units;

public class UnitGroupCreateDto : UnitGroupCreateOrUpdateDtoBase
{
    [Required]
    public IList<UnitCreateDto> Units { get; set; }
}
