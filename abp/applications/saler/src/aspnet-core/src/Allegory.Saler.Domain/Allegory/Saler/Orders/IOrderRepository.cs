using Volo.Abp.Domain.Repositories;

namespace Allegory.Saler.Orders;

public interface IOrderRepository : IRepository<Order, int>
{

}
