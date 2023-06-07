using Allegory.Saler.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Allegory.Saler.ClientUsers;

public class EfCoreClientUserRepository : EfCoreRepository<ISalerDbContext, ClientUser, int>, IClientUserRepository
{
    public EfCoreClientUserRepository(IDbContextProvider<ISalerDbContext> dbContextProvider)
    : base(dbContextProvider)
    {

    }
}
