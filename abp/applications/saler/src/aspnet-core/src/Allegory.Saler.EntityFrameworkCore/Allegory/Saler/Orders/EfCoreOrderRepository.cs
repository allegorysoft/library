using Allegory.Saler.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Allegory.Saler.Orders;

public class EfCoreOrderRepository : EfCoreRepository<ISalerDbContext, Order, int>, IOrderRepository
{
    public EfCoreOrderRepository(IDbContextProvider<ISalerDbContext> dbContextProvider) : base(dbContextProvider)
    {

    }

    public override async Task<IQueryable<Order>> WithDetailsAsync()
    {
        return (await GetQueryableAsync()).IncludeDetails();
    }
}
