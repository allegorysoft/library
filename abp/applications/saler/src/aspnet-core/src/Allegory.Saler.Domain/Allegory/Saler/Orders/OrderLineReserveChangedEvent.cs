using System.Data;

namespace Allegory.Saler.Orders;

internal class OrderLineReserveChangedEvent
{
    public OrderLine OrderLine { get; set; }
    public StatementType StatementType { get; set; }
}
