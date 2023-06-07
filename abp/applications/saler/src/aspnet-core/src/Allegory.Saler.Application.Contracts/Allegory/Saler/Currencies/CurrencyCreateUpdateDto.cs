using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Validation;

namespace Allegory.Saler.Currencies;

public class CurrencyCreateUpdateDto : EntityDto 
{
    [Required]
    [DynamicStringLength(typeof(CurrencyConsts), nameof(CurrencyConsts.MaxCodeLength))]
    public string Code { get; set; }

    [DynamicStringLength(typeof(CurrencyConsts), nameof(CurrencyConsts.MaxNameLength))]
    public string Name { get; set; }

    [DynamicStringLength(typeof(CurrencyConsts), nameof(CurrencyConsts.MaxSymbolLength
        ))]
    public string Symbol { get; set; }
}
