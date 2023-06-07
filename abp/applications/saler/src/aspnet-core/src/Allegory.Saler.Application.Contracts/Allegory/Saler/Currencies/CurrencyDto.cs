using Volo.Abp.Application.Dtos;

namespace Allegory.Saler.Currencies;

public class CurrencyDto : EntityDto<int>
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string Symbol { get; set; }
}
