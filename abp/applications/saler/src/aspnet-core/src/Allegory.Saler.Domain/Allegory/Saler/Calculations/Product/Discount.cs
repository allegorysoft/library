using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace Allegory.Saler.Calculations.Product;

public class Discount : Entity<int>
{
    #region Properties
    public int ParentId { get; protected set; }
    public decimal Rate { get; protected set; }
    public decimal Total { get; protected set; }
    #endregion

    #region Ctor
    public Discount() { }

    public Discount(decimal rate, decimal total)
    {
        Set(rate, total);
    }

    #endregion

    #region Methods
    public virtual void Set(decimal rate, decimal total)
    {
        if ((rate <= 0 || rate > 100) && total <= 0)
            throw new BusinessException(SalerDomainErrorCodes.DiscountMustBeBetweenZeroAndTotal);

        Rate = rate;
        Total = total;
    }

    public virtual void SetRate(decimal rate)
    {
        if (rate <= 0 || rate > 100)
            throw new BusinessException(SalerDomainErrorCodes.DiscountMustBeBetweenZeroAndTotal);

        Rate = rate;
    }

    public virtual void SetTotal(decimal total)
    {
        if (total <= 0)
            throw new BusinessException(SalerDomainErrorCodes.DiscountMustBeBetweenZeroAndTotal);

        Total = total;
    }

    public virtual void Calculate(decimal amount)
    {
        if (Total > 0)
            SetRate(Total / amount * 100);//(İndirim tutarı / Tutar) x 100
        else
            SetTotal(amount * Rate / 100);//Tutar * İndirim oranı / 100
    }
    #endregion
}
