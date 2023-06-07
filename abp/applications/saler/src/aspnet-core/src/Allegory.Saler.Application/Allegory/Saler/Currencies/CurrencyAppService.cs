using Allegory.Saler.Orders;
using Allegory.Saler.Permissions;
using Allegory.Saler.UnitPrices;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;

namespace Allegory.Saler.Currencies;

[Authorize(SalerPermissions.General.Currency.Default)]
public class CurrencyAppService : SalerAppService, ICurrencyAppService
{
    protected ICurrencyRepository CurrencyRepository { get; }
    protected CurrencyManager CurrencyManager { get; }
    protected ICurrencyDailyExchangeRepository CurrencyDailyExchangeRepository => LazyServiceProvider.LazyGetRequiredService<ICurrencyDailyExchangeRepository>();
    protected IUnitPriceRepository UnitPriceRepository => LazyServiceProvider.LazyGetRequiredService<IUnitPriceRepository>();
    protected IOrderRepository OrderRepository => LazyServiceProvider.LazyGetRequiredService<IOrderRepository>();
    protected IReadOnlyRepository<OrderLine, int> OrderLineRepository => LazyServiceProvider.LazyGetRequiredService<IReadOnlyRepository<OrderLine, int>>();

    public CurrencyAppService(
        ICurrencyRepository currencyRepository,
        CurrencyManager currencyManager)
    {
        CurrencyRepository = currencyRepository;
        CurrencyManager = currencyManager;
    }

    public virtual async Task<PagedResultDto<CurrencyDto>> ListAsync(FilteredPagedAndSortedResultRequestDto input)
    {
        if (string.IsNullOrEmpty(input.Sorting)) input.Sorting = nameof(Currency.Id);

        var query = await CurrencyRepository.GetQueryableAsync();

        return await query.PageResultAsync<Currency, CurrencyDto>(AsyncExecuter, ObjectMapper, input);
    }

    public virtual async Task<CurrencyDto> GetAsync(int id)
    {
        var currency = await CurrencyRepository.GetAsync(id);
        return ObjectMapper.Map<Currency, CurrencyDto>(currency);
    }

    public virtual async Task<CurrencyDto> GetByCodeAsync(string code)
    {
        var currency = await CurrencyRepository.GetByCodeAsync(code);
        return ObjectMapper.Map<Currency, CurrencyDto>(currency);
    }

    [Authorize(SalerPermissions.General.Currency.Create)]
    public virtual async Task<CurrencyDto> CreateAsync(CurrencyCreateUpdateDto input)
    {
        Currency currency = await CurrencyManager.CreateAsync(
            input.Code,
            name: input.Name,
            symbol: input.Symbol);

        await CurrencyRepository.InsertAsync(currency, autoSave: true);

        return ObjectMapper.Map<Currency, CurrencyDto>(currency);
    }

    [Authorize(SalerPermissions.General.Currency.Edit)]
    public virtual async Task<CurrencyDto> UpdateAsync(
        int id,
        CurrencyCreateUpdateDto input)
    {
        Currency currency = await CurrencyRepository.GetAsync(id);

        if (currency.Code != input.Code)
            await CurrencyManager.ChangeCodeAsync(currency, input.Code);

        currency.SetName(input.Name);
        currency.SetSymbol(input.Symbol);

        await CurrencyRepository.UpdateAsync(currency);

        return ObjectMapper.Map<Currency, CurrencyDto>(currency);
    }

    [Authorize(SalerPermissions.General.Currency.Delete)]
    public virtual async Task DeleteAsync(int id)
    {
        await CheckExistingModules(id);

        await UnitPriceRepository.DeleteAsync(x => x.CurrencyId ==  id);

        await CurrencyRepository.DeleteAsync(id);
    }
    protected virtual async Task CheckExistingModules(int currencyId)
    {
        // Check Order/Invoice/Waybill

        if (await OrderRepository.AnyAsync(order => order.CurrencyId == currencyId))
            throw new ThereIsTransactionRecordException(typeof(Currency), typeof(Order), isDelete: true);

        if (await OrderLineRepository.AnyAsync(orderLine => orderLine.CurrencyId == currencyId))
            throw new ThereIsTransactionRecordException(typeof(Currency), typeof(Order), isDelete: true);
    }

    [Authorize(SalerPermissions.General.Currency.Edit)]
    public virtual async Task EditCurrencyDailyExchangeAsync(CurrencyDailyExchangeCreateUpdateDto input)
    {
        var currencyDailyExchange = await CurrencyManager.CreateOrUpdateCurrencyDailyExchangeAsync(
            input.CurrencyCode,
            input.Date,
            input.Rate1,
            input.Rate2,
            input.Rate3,
            input.Rate4);

        if (currencyDailyExchange.Id == default)
            await CurrencyDailyExchangeRepository.InsertAsync(currencyDailyExchange, autoSave: true);
        else
            await CurrencyDailyExchangeRepository.UpdateAsync(currencyDailyExchange);
    }

    public virtual async Task<CurrencyDailyExchangeDto> GetCurrencyDailyExchangeAsync(
        string currencyCode,
        DateTime date)
    {
        var query = from c in await CurrencyRepository.GetQueryableAsync()
                    join cd in await CurrencyDailyExchangeRepository.GetQueryableAsync()
                    on new { Id = c.Id, Date = date.Date }
                        equals new { Id = cd.CurrencyId, Date = cd.Date }
                        into cds
                    from cd in cds.DefaultIfEmpty()
                    where c.Code == currencyCode
                    select new { CurrencyCode = c.Code, CurrencyDailyExchange = cd };

        var result = await AsyncExecuter.FirstOrDefaultAsync(query);

        if (result == null)
            throw new CodeNotFoundException(typeof(Currency), currencyCode);

        CurrencyDailyExchangeDto output;
        if (result.CurrencyDailyExchange != null)
        {
            output = ObjectMapper.Map<CurrencyDailyExchange, CurrencyDailyExchangeDto>(result.CurrencyDailyExchange);
            output.CurrencyCode = result.CurrencyCode;
        }
        else
            output = new CurrencyDailyExchangeDto()
            {
                CurrencyCode = result.CurrencyCode,
                Rate1 = 1,
                Rate2 = 1,
                Rate3 = 1,
                Rate4 = 1,
            };

        return output;

    }

    public virtual async Task<IList<CurrencyDailyExchangeDto>> GetCurrencyDailyExchangeListAsync(DateTime date)
    {
        var query = from c in await CurrencyRepository.GetQueryableAsync()
                    join cd in await CurrencyDailyExchangeRepository.GetQueryableAsync()
                   on new { Id = c.Id, Date = date.Date }
                        equals new { Id = cd.CurrencyId, Date = cd.Date }
                        into cds
                    from cd in cds.DefaultIfEmpty()

                    select new CurrencyDailyExchangeDto
                    {
                        CurrencyCode = c.Code,
                        Rate1 = cd == null ? 0 : cd.Rate1,
                        Rate2 = cd == null ? 0 : cd.Rate2,
                        Rate3 = cd == null ? 0 : cd.Rate3,
                        Rate4 = cd == null ? 0 : cd.Rate4,
                    };

        var result = await AsyncExecuter.ToListAsync(query);
        return result;
    }

    [Authorize(SalerPermissions.General.Currency.Edit)]
    public virtual async Task RefreshDailyExchangesAsync()
    {
        await CurrencyManager.RefreshDailyExchangesAsync();
    }
}
