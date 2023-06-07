using Allegory.Saler.Calculations.Product;
using System;
using Volo.Abp;

namespace Allegory.Saler.Orders;

public class OrderLine : CalculableProduct<OrderLineDiscount>
{
    public int OrderId { get; protected set; }
    public OrderLineType Type { get; internal set; }
    public int ProductId { get; internal set; }
    public int UnitId { get; internal set; }
    public DateTime? ReserveDate { get; protected set; }
    public decimal? ReserveQuantity { get; protected set; }

    internal OrderLine() { }

    internal void CheckQuantity()
    {
        if (Quantity <= 0)
            throw new BusinessException(SalerDomainErrorCodes.QuantityCannotZeroOrLess);
    }

    internal void SetReserve(
        DateTime? reserveDate,
        decimal? reserveQuantity = default)
    {
        if (reserveDate.HasValue)
        {
            if (!reserveQuantity.HasValue
                || reserveQuantity.Value > Quantity
                || reserveQuantity.Value <= 0)
                throw new BusinessException(SalerDomainErrorCodes.ReserveQuantityMustGreaterThanZeroAndLessThanQuantity);
        }
        else
        {
            if (reserveQuantity.HasValue)
                throw new BusinessException(SalerDomainErrorCodes.ReserveWrong);
        }

        ReserveDate = reserveDate;
        ReserveQuantity = reserveQuantity;
    }

    internal bool IsReserveChange(
        DateTime? reserveDate,
        decimal? reserveQuantity,
        int unitId,
        int productId)
    {
        return
            ReserveDate != reserveDate
            || (reserveDate.HasValue
                &&    (UnitId != unitId
                    || ProductId != productId
                    || ReserveQuantity != reserveQuantity));
    }
}
