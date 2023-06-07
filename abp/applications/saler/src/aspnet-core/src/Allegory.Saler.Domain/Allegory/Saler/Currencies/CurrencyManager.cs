using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Allegory.Saler.Currencies;

public class CurrencyManager : SalerDomainService
{
    protected ICurrencyRepository CurrencyRepository { get; }
    protected IDailyExchangeService DailyExchangeService => LazyServiceProvider.LazyGetRequiredService<IDailyExchangeService>();
    protected ICurrencyDailyExchangeRepository CurrencyDailyExchangeRepository => LazyServiceProvider.LazyGetRequiredService<ICurrencyDailyExchangeRepository>();

    public CurrencyManager(ICurrencyRepository currencyRepository)
    {
        CurrencyRepository = currencyRepository;
    }

    public async Task<Currency> CreateAsync(
        string code,
        string name = default,
        string symbol = default)
    {
        await CheckCurrencyExistsAsync(code);

        var currency = new Currency(
            code,
            name: name,
            symbol: symbol);

        return currency;
    }

    public async Task CheckCurrencyExistsAsync(
        string code,
        int? id = default)
    {
        Expression<Func<Currency, bool>> expression = currency => currency.Code == code;

        if (id != default)
            expression = expression.And(item => item.Id != id);

        var currencyExists = await CurrencyRepository.AnyAsync(expression);

        if (currencyExists)
            throw new CodeAlreadyExistsException(typeof(Currency), code);
    }

    public async Task ChangeCodeAsync(
        Currency currency,
        string newCode)
    {
        await CheckCurrencyExistsAsync(newCode, currency.Id);
        currency.ChangeCode(newCode);
    }

    public async Task<CurrencyDailyExchange> CreateOrUpdateCurrencyDailyExchangeAsync(
        string currencyCode,
        DateTime date,
        decimal rate1,
        decimal rate2,
        decimal rate3,
        decimal rate4)
    {
        var currency = await CurrencyRepository.GetByCodeAsync(
            currencyCode,
            includeDetails: false);

        var currencyDailyExchange = await CurrencyDailyExchangeRepository.FindAsync(c => c.CurrencyId == currency.Id && c.Date == date);

        if (currencyDailyExchange == null)
            currencyDailyExchange = new CurrencyDailyExchange(currency.Id, date);

        currencyDailyExchange.SetRate1(rate1);
        currencyDailyExchange.SetRate2(rate2);
        currencyDailyExchange.SetRate3(rate3);
        currencyDailyExchange.SetRate4(rate4);

        return currencyDailyExchange;
    }

    public async Task RefreshDailyExchangesAsync()
    {
        var exchanges = await DailyExchangeService.GetDailyExchangesAsync();
        List<CurrencyDailyExchange> currencyDailyExchanges = new();
        foreach (var exchange in exchanges)
        {
            try
            {
                currencyDailyExchanges.Add(
                await CreateOrUpdateCurrencyDailyExchangeAsync(
                    exchange.CurrencyCode,
                    DateTime.Now,
                    exchange.Rate1,
                    exchange.Rate2,
                    exchange.Rate3,
                    exchange.Rate4));
            }
            catch (CodeNotFoundException) { }
        }

        await CurrencyDailyExchangeRepository.InsertManyAsync(
            currencyDailyExchanges.Where(x => x.Id == default));
        await CurrencyDailyExchangeRepository.UpdateManyAsync(
            currencyDailyExchanges.Where(x => x.Id != default));
    }
}
