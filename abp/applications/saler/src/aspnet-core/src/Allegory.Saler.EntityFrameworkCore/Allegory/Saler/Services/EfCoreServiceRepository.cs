using Allegory.Saler.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Allegory.Saler.Services;

public class EfCoreServiceRepository : EfCoreRepository<ISalerDbContext, Service, int>, IServiceRepository
{
    public EfCoreServiceRepository(IDbContextProvider<ISalerDbContext> dbContextProvider)
        : base(dbContextProvider)
    {

    }
}
