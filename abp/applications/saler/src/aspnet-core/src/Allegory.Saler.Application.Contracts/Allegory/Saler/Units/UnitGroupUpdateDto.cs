using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Allegory.Saler.Units;

public class UnitGroupUpdateDto : UnitGroupCreateOrUpdateDtoBase
{
    [Required]
    public IList<UnitUpdateDto> Units { get; set; }
}
