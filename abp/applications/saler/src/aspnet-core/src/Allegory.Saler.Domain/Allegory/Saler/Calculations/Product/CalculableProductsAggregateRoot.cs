using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Allegory.Saler.Calculations.Product;

public class CalculableProductsAggregateRoot<AD, C, D> : AuditedAggregateRoot<int>
    where AD : Discount
    where C : CalculableProduct<D>
    where D : Discount
{
    #region Properties
    public decimal TotalDiscount { get; protected set; }
    public decimal TotalVatBase { get; protected set; }
    public decimal TotalVatAmount { get; protected set; }
    public decimal TotalGross { get; protected set; }
    public IList<C> Lines { get; protected set; }
    public IList<AD> Discounts { get; protected set; }
    public int? CurrencyId { get; internal set; }
    public decimal? CurrencyRate { get; protected set; }
    #endregion

    #region Ctor
    public CalculableProductsAggregateRoot()
    {
        Lines = new Collection<C>();
        Discounts = new Collection<AD>();
    }
    #endregion

    #region Methods
    internal virtual void SetTotalDiscount(decimal discount)
    {
        if (discount < 0)
            throw new BusinessException(SalerDomainErrorCodes.DiscountMustBeBetweenZeroAndTotal);

        TotalDiscount = discount;
    }

    internal virtual void SetTotalVatBase(decimal totalVatBase)
    {
        if (totalVatBase < 0)
            throw new BusinessException(SalerDomainErrorCodes.VatMustBeBetweenZeroAndTotal);

        TotalVatBase = totalVatBase;
    }

    internal virtual void SetTotalVatAmount(decimal totalVatAmount)
    {
        if (totalVatAmount < 0)
            throw new BusinessException(SalerDomainErrorCodes.VatMustBeBetweenZeroAndTotal);

        TotalVatAmount = totalVatAmount;
    }

    internal virtual void SetTotalGross(decimal totalGross)
    {
        if (totalGross < 0)
            throw new BusinessException(SalerDomainErrorCodes.TotalCannotLessThanZero);

        TotalGross = totalGross;
    }

    internal virtual void SetCurrencyRate(decimal? currencyRate)
    {
        if (currencyRate <= 0)
            throw new BusinessException(SalerDomainErrorCodes.CurrencyRateMustBeGreaterThanZero);

        CurrencyRate = currencyRate;
    }
    #endregion
}