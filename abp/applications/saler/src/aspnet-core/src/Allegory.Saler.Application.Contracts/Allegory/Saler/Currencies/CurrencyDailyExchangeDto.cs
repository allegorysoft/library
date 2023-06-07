using System;
using Volo.Abp.Application.Dtos;

namespace Allegory.Saler.Currencies;

public class CurrencyDailyExchangeDto : EntityDto
{
    public string CurrencyCode { get; set; }
    public decimal Rate1 { get; set; }
    public decimal Rate2 { get; set; }
    public decimal Rate3 { get; set; }
    public decimal Rate4 { get; set; }
}
