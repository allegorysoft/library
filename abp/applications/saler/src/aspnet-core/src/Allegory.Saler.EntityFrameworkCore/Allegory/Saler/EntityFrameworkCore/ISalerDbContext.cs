using Allegory.Saler.Clients;
using Allegory.Saler.ClientUsers;
using Allegory.Saler.Currencies;
using Allegory.Saler.Items;
using Allegory.Saler.Orders;
using Allegory.Saler.Services;
using Allegory.Saler.UnitPrices;
using Allegory.Saler.Units;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Allegory.Saler.EntityFrameworkCore
{
    [ConnectionStringName(SalerConsts.ConnectionStringName)]
    public interface ISalerDbContext : IEfCoreDbContext
    {
        DbSet<UnitGroup> UnitGroups { get; }
        DbSet<Unit> Units { get; set; }
        DbSet<Client> Clients { get; }
        DbSet<ClientUser> ClientUsers { get; }
        DbSet<Item> Items { get; }
        DbSet<ItemStockTransaction> ItemStockTransactions { get; }
        DbSet<Service> Services { get; }
        DbSet<Order> Orders { get; }
        DbSet<OrderLine> OrderLines { get; }
        DbSet<OrderLineDiscount> OrderLineDiscounts { get; }
        DbSet<Currency> Currencies { get; }
        DbSet<CurrencyDailyExchange> CurrencyDailyExchanges { get; }
        DbSet<UnitPrice> UnitPrices { get; }
    }
}
