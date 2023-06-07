using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace Allegory.Saler.Currencies;

public class CurrencyDailyExchange : Entity<int>
{
    public int CurrencyId { get; protected set; }
    public DateTime Date { get; set; }
    public decimal Rate1 { get; protected set; }
    public decimal Rate2 { get; protected set; }
    public decimal Rate3 { get; protected set; }
    public decimal Rate4 { get; protected set; }

    protected CurrencyDailyExchange() { }

    internal CurrencyDailyExchange(
        int currencyId,
        DateTime date)
    {
        CurrencyId = currencyId;
        Date = date;
    }

    public void SetRate1(decimal rate)
    {
        if (rate < 0)
            throw new BusinessException(SalerDomainErrorCodes.CurrencyRateMustBeGreaterThanZero);
        Rate1 = rate;
    }

    public void SetRate2(decimal rate)
    {
        if (rate < 0)
            throw new BusinessException(SalerDomainErrorCodes.CurrencyRateMustBeGreaterThanZero);

        Rate2 = rate;
    }

    public void SetRate3(decimal rate)
    {
        if (rate < 0)
            throw new BusinessException(SalerDomainErrorCodes.CurrencyRateMustBeGreaterThanZero);

        Rate3 = rate;
    }

    public void SetRate4(decimal rate)
    {
        if (rate < 0)
            throw new BusinessException(SalerDomainErrorCodes.CurrencyRateMustBeGreaterThanZero);

        Rate4 = rate;
    }
}
