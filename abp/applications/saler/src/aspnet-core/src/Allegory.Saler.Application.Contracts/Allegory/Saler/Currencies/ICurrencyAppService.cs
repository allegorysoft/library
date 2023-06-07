using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Allegory.Saler.Currencies;

public interface ICurrencyAppService : IApplicationService
{
    Task<PagedResultDto<CurrencyDto>> ListAsync(FilteredPagedAndSortedResultRequestDto input);
    Task<CurrencyDto> GetAsync(int id);
    Task<CurrencyDto> GetByCodeAsync(string code);
    Task<CurrencyDto> CreateAsync(CurrencyCreateUpdateDto input);
    Task<CurrencyDto> UpdateAsync(int id, CurrencyCreateUpdateDto input);
    Task DeleteAsync(int id);
    Task EditCurrencyDailyExchangeAsync(CurrencyDailyExchangeCreateUpdateDto input);
    Task<CurrencyDailyExchangeDto> GetCurrencyDailyExchangeAsync(
        string currencyCode,
        DateTime date);
    Task<IList<CurrencyDailyExchangeDto>> GetCurrencyDailyExchangeListAsync(DateTime date);
    Task RefreshDailyExchangesAsync();
}
