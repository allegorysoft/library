using Allegory.Saler.Calculations.Product;
using System;
using Volo.Abp;

namespace Allegory.Saler.Orders;

public class Order : CalculableProductsAggregateRoot<OrderDiscount, OrderLine, OrderLineDiscount>
{
    public string Number { get; protected set; }
    public OrderType Type { get; private set; }
    public OrderStatu Statu { get; internal set; }
    public int? ClientId { get; set; }
    public DateTime Date { get; set; }

    protected Order() { }

    internal Order(
        string number,
        OrderType type,
        DateTime date,
        OrderStatu statu = OrderStatu.Offer,
        int? clientId = default)
    {
        SetNumber(number);
        Type = type;
        Date = date;
        Statu = statu;
        ClientId = clientId;
    }

    internal Order ChangeNumber(string number)
    {
        SetNumber(number);
        return this;
    }
    private void SetNumber(string number)
    {
        Check.NotNullOrWhiteSpace(number, nameof(Number), OrderConsts.MaxNumberLength);
        Number = number;
    }

    public void SetStatu(OrderStatu statu)
    {
        if (Statu == statu)
            return;

        Statu = statu;

        AddLocalEvent(
            new OrderStatuChangedEvent
            {
                Order = this
            });
    }
}
