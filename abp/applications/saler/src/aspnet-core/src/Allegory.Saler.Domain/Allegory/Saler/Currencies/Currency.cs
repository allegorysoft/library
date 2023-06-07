using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace Allegory.Saler.Currencies;

public class Currency : AggregateRoot<int>, ICode
{
    public string Code { get; protected set; }
    public string Name { get; protected set; }
    public string Symbol { get; protected set; }

    protected Currency() { }

    internal Currency(
        string code,
        string name = default,
        string symbol = default)
    {
        SetCode(code);
        SetName(name);
        SetSymbol(symbol);
    }

    internal Currency ChangeCode(string code)
    {
        SetCode(code);
        return this;
    }

    private void SetCode(string code)
    {
        Check.NotNullOrWhiteSpace(code, nameof(Code), CurrencyConsts.MaxCodeLength);
        Code = code;
    }

    public void SetName(string name)
    {
        Check.Length(name, nameof(Name), CurrencyConsts.MaxNameLength);
        Name = name;
    }

    public void SetSymbol(string symbol)
    {
        Check.Length(symbol, nameof(Symbol), CurrencyConsts.MaxSymbolLength);
        Symbol = symbol;
    }
}
