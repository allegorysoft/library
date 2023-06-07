using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace Allegory.Saler.UnitPrices;

public class UnitPrice : AuditedAggregateRoot<int>
{
    public string Code { get; protected set; }
    public UnitPriceType Type { get; protected set; }
    public int ProductId { get; internal set; }
    public int UnitId { get; internal set; }
    public bool IsVatIncluded { get; set; }
    public decimal PurchasePrice { get; protected set; }
    public decimal SalesPrice { get; protected set; }
    public DateTime BeginDate { get; protected set; }
    public DateTime EndDate { get; protected set; }
    public int? CurrencyId { get; internal set; }
    public int? ClientId { get; internal set; }
    
    internal UnitPrice(
        string code,
        UnitPriceType type)
    {
        SetCode(code);
        Type = type;
    }

    internal UnitPrice ChangeCode(string code)
    {
        SetCode(code);
        return this;
    }

    private void SetCode(string code)
    {
        Check.NotNullOrWhiteSpace(code, nameof(Code), UnitPriceConsts.MaxCodeLength);
        Code = code;
    }

    public void SetPrice(
        decimal purchasePrice,
        decimal salesPrice)
    {
        if (purchasePrice < 0 || salesPrice < 0)
            throw new BusinessException(SalerDomainErrorCodes.PriceCannotLessThanZero);

        PurchasePrice = purchasePrice;
        SalesPrice = salesPrice;
    }

    public void SetDates(DateTime beginDate, DateTime endDate)
    {
        if (beginDate > endDate)
            throw new BusinessException(SalerDomainErrorCodes.BeginDateCannotBeGreaterThanEndDate);

        BeginDate = beginDate;
        EndDate = endDate;
    }
}
