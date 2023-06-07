using Allegory.Saler.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Allegory.Saler.UnitPrices;

public class EfCoreUnitPriceRepository : EfCoreRepository<ISalerDbContext, UnitPrice, int>, IUnitPriceRepository
{
    public EfCoreUnitPriceRepository(IDbContextProvider<ISalerDbContext> dbContextProvider) : base(dbContextProvider)
    {

    }
}
