using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Validation;

namespace Allegory.Saler.Currencies;

public class CurrencyDailyExchangeCreateUpdateDto : EntityDto
{
    [Required]
    [DynamicStringLength(typeof(CurrencyConsts), nameof(CurrencyConsts.MaxCodeLength))]
    public string CurrencyCode { get; set; }

    [Required]
    public DateTime Date { get; set; }

    public decimal Rate1 { get; set; }

    public decimal Rate2 { get; set; }

    public decimal Rate3 { get; set; }

    public decimal Rate4 { get; set; }
}
