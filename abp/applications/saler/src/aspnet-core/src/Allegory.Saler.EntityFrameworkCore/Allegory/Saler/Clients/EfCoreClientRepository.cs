using Allegory.Saler.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Allegory.Saler.Clients;

public class EfCoreClientRepository : EfCoreRepository<ISalerDbContext, Client, int>, IClientRepository
{
    public EfCoreClientRepository(IDbContextProvider<ISalerDbContext> dbContextProvider)
        : base(dbContextProvider)
    {

    }
}
