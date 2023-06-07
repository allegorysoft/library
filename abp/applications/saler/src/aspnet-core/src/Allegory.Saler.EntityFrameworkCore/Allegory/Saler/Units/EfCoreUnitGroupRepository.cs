using Allegory.Saler.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Allegory.Saler.Units;

public class EfCoreUnitGroupRepository : EfCoreRepository<ISalerDbContext, UnitGroup, int>, IUnitGroupRepository
{
    public EfCoreUnitGroupRepository(IDbContextProvider<ISalerDbContext> dbContextProvider)
        : base(dbContextProvider)
    {

    }

    public override async Task<IQueryable<UnitGroup>> WithDetailsAsync()
    {
        return (await GetQueryableAsync()).IncludeDetails();
    }
}
