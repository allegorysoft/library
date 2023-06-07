using Allegory.Saler.Clients;
using Allegory.Saler.ClientUsers;
using Allegory.Saler.Currencies;
using Allegory.Saler.Items;
using Allegory.Saler.Orders;
using Allegory.Saler.Services;
using Allegory.Saler.UnitPrices;
using Allegory.Saler.Units;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.IdentityServer.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;

namespace Allegory.Saler.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ISalerDbContext))]//When we use default repositories it create SalerDbContext(concrete db context) but our repositories uses ISalerDbContext(abstract db context) and it throws multiple db context exception on join queries, so we say if abstract db context requested use concrete db context solve problem
[ConnectionStringName(SalerConsts.ConnectionStringName)]
public class SalerDbContext :
    AbpDbContext<SalerDbContext>,
    IIdentityDbContext,
    ISalerDbContext
{
    public DbSet<UnitGroup> UnitGroups { get; set; }
    public DbSet<Unit> Units { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<ClientUser> ClientUsers { get; }
    public DbSet<Item> Items { get; set; }
    public DbSet<ItemStockTransaction> ItemStockTransactions { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderLine> OrderLines { get; set; }
    public DbSet<OrderLineDiscount> OrderLineDiscounts { get; set; }
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<CurrencyDailyExchange> CurrencyDailyExchanges { get; set; }
    public DbSet<UnitPrice> UnitPrices { get; set; }

    #region Entities from the modules

    /* Notice: We only implemented IIdentityDbContext and ITenantManagementDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityDbContext and ITenantManagementDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    //Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }

    #endregion

    public SalerDbContext(DbContextOptions<SalerDbContext> options)
        : base(options)
    {
        //ChangeTracker.AutoDetectChangesEnabled = false; //when you seed big data open this line
        //https://docs.microsoft.com/en-us/ef/core/querying/single-split-queries
        //Collection query behavior may be changed default is split query
    }

#if DEBUG
    public static readonly ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder
        .AddFilter((category, level) =>
            category == DbLoggerCategory.Database.Command.Name
             &&
            level == LogLevel.Information)
        .AddConsole());
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.UseLoggerFactory(loggerFactory);
        base.OnConfiguring(optionsBuilder);
    }
#endif

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentity();
        builder.ConfigureIdentityServer();
        builder.ConfigureSaler();
    }
}
