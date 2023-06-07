using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace Allegory.Saler.Calculations.Product;

public class CalculableProduct<D> : Entity<int>, IDeduction
    where D : Discount
{
    #region Properties
    public decimal Quantity { get; protected set; }
    public decimal Price { get; protected set; }
    public decimal VatRate { get; protected set; }
    public bool IsVatIncluded { get; internal set; }
    public decimal Total { get; protected set; }

    public decimal LineVatExcludedTotal => IsVatIncluded
        ? Total / (1 + (VatRate / 100))
        : Total;
    public decimal LineVatIncludedTotal => IsVatIncluded
        ? Total
        : Total * (1 + (VatRate / 100));

    public decimal DiscountTotal { get; protected set; }
    public decimal CalculatedTotal { get; protected set; }

    public decimal VatBase { get; protected set; }
    public decimal VatAmount { get; protected set; }

    public IList<D> Discounts { get; protected set; }

    public short? DeductionPart1 { get; protected set; }
    public short? DeductionPart2 { get; protected set; }
    public string DeductionCode { get; protected set; }

    #region Currency
    public int? CurrencyId { get; internal set; }
    public decimal? CurrencyRate { get; protected set; }
    public decimal? CurrencyPrice { get; protected set; }
    public decimal? CurrencyTotal { get; protected set; }
    #endregion

    #endregion

    #region Ctor
    internal CalculableProduct()
    {
        Discounts = new Collection<D>();
    }

    internal CalculableProduct(
        decimal quantity,
        decimal price,
        decimal vatRate,
        bool isVatIncluded,
        decimal total = default,
        string deductionCode = default,
        short? deductionPart1 = default,
        short? deductionPart2 = default)
    {
        Discounts = new Collection<D>();

        SetCalculateInput(
            quantity,
            price: price,
            vatRate: vatRate,
            isVatIncluded: isVatIncluded,
            total: total,
            deductionCode: deductionCode,
            deductionPart1: deductionPart1,
            deductionPart2: deductionPart2);
    }
    #endregion

    #region Methods
    internal virtual void SetQuantity(decimal quantity)
    {
        if (quantity < 0)
            throw new BusinessException(SalerDomainErrorCodes.QuantityCannotZeroOrLess);

        Quantity = quantity;
    }

    internal virtual void SetPrice(decimal price)
    {
        if (price < 0)
            throw new BusinessException(SalerDomainErrorCodes.PriceCannotLessThanZero);

        Price = price;
    }

    internal virtual void SetVatRate(decimal vatRate)
    {
        if (vatRate < 0 || vatRate > 100)
            throw new BusinessException(SalerDomainErrorCodes.VatRateMustBeBetweenZeroAndOneHundred);

        VatRate = vatRate;
    }

    internal virtual void SetVatBase(decimal vatBase)
    {
        if (vatBase < 0)
            throw new BusinessException(SalerDomainErrorCodes.VatMustBeBetweenZeroAndTotal);

        VatBase = vatBase;
    }

    internal virtual void SetVatAmount(decimal vatAmount)
    {
        if (vatAmount < 0)
            throw new BusinessException(SalerDomainErrorCodes.VatMustBeBetweenZeroAndTotal);

        VatAmount = vatAmount;
    }

    internal virtual void SetTotal(decimal total)
    {
        if (total < 0)
            throw new BusinessException(SalerDomainErrorCodes.TotalCannotLessThanZero);

        Total = total;
    }

    internal virtual void SetDiscountTotal(decimal discount)
    {
        if (discount < 0 || discount > Total)
            throw new BusinessException(SalerDomainErrorCodes.DiscountMustBeBetweenZeroAndTotal);

        DiscountTotal = discount;
    }

    internal virtual void SetCalculatedTotal(decimal calculatedTotal)
    {
        if (calculatedTotal < 0)
            throw new BusinessException(SalerDomainErrorCodes.TotalCannotLessThanZero);

        CalculatedTotal = calculatedTotal;
    }

    internal virtual void SetCurrencyRate(decimal? currencyRate)
    {
        if (currencyRate <= 0)
            throw new BusinessException(SalerDomainErrorCodes.CurrencyRateMustBeGreaterThanZero);

        CurrencyRate = currencyRate;
    }

    internal virtual void SetCurrencyPrice(decimal? currencyPrice)
    {
        if (currencyPrice < 0)
            throw new BusinessException(SalerDomainErrorCodes.PriceCannotLessThanZero);

        CurrencyPrice = currencyPrice;
    }

    internal virtual void SetCurrencyTotal(decimal? currencyTotal)
    {
        if (currencyTotal < 0)
            throw new BusinessException(SalerDomainErrorCodes.TotalCannotLessThanZero);

        CurrencyTotal = currencyTotal;
    }

    internal virtual void SetCalculateInput(
        decimal quantity,
        decimal price = default,
        decimal vatRate = default,
        bool isVatIncluded = default,
        decimal total = default,
        string deductionCode = default,
        short? deductionPart1 = default,
        short? deductionPart2 = default)
    {
        SetQuantity(quantity);
        SetPrice(price);
        SetVatRate(vatRate);
        IsVatIncluded = isVatIncluded;
        SetTotal(total);
        DeductionCode = deductionCode;
        DeductionPart1 = deductionPart1;
        DeductionPart2 = deductionPart2;
    }

    public CalculableProduct<D> Clone()
    {
        return (CalculableProduct<D>)MemberwiseClone();
    }
    #endregion
}
