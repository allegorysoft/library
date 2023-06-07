using Allegory.Saler.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Allegory.Saler.Items;

public class EfCoreItemRepository : EfCoreRepository<ISalerDbContext, Item, int>, IItemRepository
{
    public EfCoreItemRepository(IDbContextProvider<ISalerDbContext> dbContextProvider) 
        : base(dbContextProvider)
    {

    }
}
