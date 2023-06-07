using Allegory.Saler.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Allegory.Saler.Currencies;

public class EfCoreCurrencyRepository : EfCoreRepository<ISalerDbContext, Currency, int>, ICurrencyRepository
{
    public EfCoreCurrencyRepository(IDbContextProvider<ISalerDbContext> dbContextProvider) 
        : base(dbContextProvider)
    {

    }
}
