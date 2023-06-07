using Allegory.Saler.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Allegory.Saler.Currencies;

public class EfCoreCurrencyDailyExchangeRepository : EfCoreRepository<ISalerDbContext, CurrencyDailyExchange, int>, ICurrencyDailyExchangeRepository
{
    public EfCoreCurrencyDailyExchangeRepository(IDbContextProvider<ISalerDbContext> dbContextProvider)
        : base(dbContextProvider)
    {

    }
}
