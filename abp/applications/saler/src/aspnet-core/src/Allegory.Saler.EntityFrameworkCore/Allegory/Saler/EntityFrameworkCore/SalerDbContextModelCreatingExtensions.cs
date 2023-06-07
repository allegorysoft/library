using Allegory.Saler.Calculations.Product;
using Allegory.Saler.Clients;
using Allegory.Saler.ClientUsers;
using Allegory.Saler.Currencies;
using Allegory.Saler.Items;
using Allegory.Saler.Orders;
using Allegory.Saler.Services;
using Allegory.Saler.UnitPrices;
using Allegory.Saler.Units;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.Identity;

namespace Allegory.Saler.EntityFrameworkCore
{
    public static class SalerDbContextModelCreatingExtensions
    {
        public static void ConfigureSaler(this ModelBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            builder.Entity<Unit>(b =>
            {
                b.ToTable(SalerConsts.DbTablePrefix + "Units", SalerConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.Code)
                 .IsRequired()
                 .HasMaxLength(UnitConsts.MaxCodeLength);
                b.Property(x => x.Name)
                 .HasMaxLength(UnitConsts.MaxNameLength);
                b.Property(x => x.MainUnit).IsRequired();
                b.Property(x => x.ConvFact1)
                 .HasColumnType("float")
                 .IsRequired();
                b.Property(x => x.ConvFact2)
                 .HasColumnType("float")
                 .IsRequired();
                b.Property(x => x.Divisible).IsRequired();
                b.Property(x => x.GlobalUnitCode)
                 .HasMaxLength(UnitConsts.MaxGlobalUnitCodeLength);

                b.HasIndex(x => new
                {
                    x.UnitGroupId,
                    x.Code
                }).IsUnique(true);
            });

            builder.Entity<UnitGroup>(b =>
            {
                b.ToTable(SalerConsts.DbTablePrefix + "UnitGroups", SalerConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.Code)
                 .IsRequired()
                 .HasMaxLength(UnitGroupConsts.MaxCodeLength);
                b.Property(x => x.Name)
                 .HasMaxLength(UnitGroupConsts.MaxNameLength);

                b.HasMany(x => x.Units)
                 .WithOne()
                 .HasForeignKey(uc => uc.UnitGroupId)
                 .IsRequired();

                b.HasIndex(x => x.Code)
                 .IsUnique(true);
            });

            builder.Entity<Client>(b =>
            {
                b.ToTable(SalerConsts.DbTablePrefix + "Clients", SalerConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.Type).IsRequired(true);
                b.Property(x => x.Code)
                 .IsRequired()
                 .HasMaxLength(ClientConsts.MaxCodeLength);
                b.Property(x => x.Title)
                 .HasMaxLength(ClientConsts.MaxTitleLength);
                b.Property(x => x.IdentityNumber)
                 .HasMaxLength(ClientConsts.MaxIdentityNumberLength);
                b.Property(x => x.Name)
                 .HasMaxLength(ClientConsts.MaxNameLength);
                b.Property(x => x.Surname)
                 .HasMaxLength(ClientConsts.MaxSurnameLength);
                b.Property(x => x.TaxOffice)
                 .HasMaxLength(ClientConsts.MaxTaxOfficeLength);
                b.Property(x => x.Phone1)
                 .HasMaxLength(ClientConsts.MaxPhoneLength);
                b.Property(x => x.Phone2)
                 .HasMaxLength(ClientConsts.MaxPhoneLength);
                b.Property(x => x.Phone3)
                 .HasMaxLength(ClientConsts.MaxPhoneLength);
                b.Property(x => x.EMail)
                 .HasMaxLength(ClientConsts.MaxMailLength);
                b.Property(x => x.KepAddress)
                 .HasMaxLength(ClientConsts.MaxMailLength);
                
                b.HasIndex(x => x.Code).IsUnique(true);
                b.HasIndex(x => x.IdentityNumber).IsUnique(false);
            });

            builder.Entity<ClientUser>(b =>
            {
                b.ToTable(SalerConsts.DbTablePrefix + "ClientUsers", SalerConsts.DbSchema);
                b.ConfigureByConvention();

                b.HasOne<Client>()
                 .WithMany()
                 .HasForeignKey(x => x.ClientId)
                 .IsRequired()
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasOne<IdentityUser>()
                 .WithMany()
                 .HasForeignKey(x => x.UserId)
                 .IsRequired()
                 .OnDelete(DeleteBehavior.NoAction);

                b.HasIndex(x => new
                {
                    x.ClientId,
                    x.UserId
                }).IsUnique(true);
            });

            builder.Entity<Item>(b =>
            {
                b.ToTable(SalerConsts.DbTablePrefix + "Items", SalerConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.Code)
                 .IsRequired()
                 .HasMaxLength(ItemConsts.MaxCodeLength);
                b.Property(x => x.Name)
                 .HasMaxLength(ItemConsts.MaxNameLength);
                b.Property(x => x.Type).IsRequired();
                b.Property(x => x.SalesVatRate).IsRequired();
                b.Property(x => x.PurchaseVatRate).IsRequired();
                b.Property(x => x.DeductionCode)
                 .HasMaxLength(DeductionConsts.MaxDeductionCodeLength);

                b.HasOne<UnitGroup>()
                 .WithMany()
                 .HasForeignKey(x => x.UnitGroupId)
                 .IsRequired()
                 .OnDelete(DeleteBehavior.NoAction);

                b.HasIndex(x => x.Code)
                 .IsUnique(true);

            });

            builder.Entity<ItemStockTransaction>(b =>
            {
                b.ToTable(SalerConsts.DbTablePrefix + "ItemStockTransactions", SalerConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.Quantity)
                 .HasColumnType("float")
                 .IsRequired();
                b.Property(x => x.Date)
                 .HasColumnType("date")
                 .IsRequired();
                b.Property(x => x.Type).IsRequired();
                b.Property(x => x.TransactionId).IsRequired();
                b.Property(x => x.TransactionParentId).IsRequired();
                b.Property(x => x.IsOutput).IsRequired();
                b.Property(x => x.Statu).IsRequired();

                b.HasOne<Item>()
                 .WithMany()
                 .HasForeignKey(x => x.ItemId)
                 .IsRequired()
                 .OnDelete(DeleteBehavior.NoAction);

                b.HasIndex(x => new
                {
                    x.Type,
                    x.TransactionId,
                    x.IsOutput
                }).IsUnique(true);

                b.HasIndex(x => new
                {
                    x.Type,
                    x.TransactionParentId,
                }).IsUnique(false);
            });

            builder.Entity<Service>(b =>
            {
                b.ToTable(SalerConsts.DbTablePrefix + "Services", SalerConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.Code)
                 .IsRequired()
                 .HasMaxLength(ServiceConsts.MaxCodeLength);
                b.Property(x => x.Name)
                 .HasMaxLength(ServiceConsts.MaxNameLength);
                b.Property(x => x.SalesVatRate).IsRequired();
                b.Property(x => x.PurchaseVatRate).IsRequired();
                b.Property(x => x.DeductionCode)
                 .HasMaxLength(DeductionConsts.MaxDeductionCodeLength);

                b.HasOne<UnitGroup>()
                 .WithMany()
                 .HasForeignKey(x => x.UnitGroupId)
                 .IsRequired()
                 .OnDelete(DeleteBehavior.NoAction);

                b.HasIndex(x => x.Code)
                 .IsUnique(true);
            });

            builder.Entity<OrderLine>(b =>
            {
                b.Property(x => x.Type).IsRequired();
                b.Property(x => x.ProductId).IsRequired();
                b.Property(x => x.UnitId).IsRequired();
                b.Property(x => x.ReserveQuantity).HasColumnType("float");
                b.Property(x => x.ReserveDate).HasColumnType("date");

                b.HasOne<Unit>()
                 .WithMany()
                 .HasForeignKey(x => x.UnitId)
                 .IsRequired()
                 .OnDelete(DeleteBehavior.NoAction);

                b.HasIndex(x => new
                {
                    x.Type,
                    x.ProductId
                }).IsUnique(false);

                //TODO "Check Existing Data On Create = No", "Enforce Foreign Key Constraint = No"
                //b.HasOne<Item>()
                // .WithMany()
                // .HasForeignKey(x => x.productId)
                // .IsRequired(false)
                // .OnDelete(DeleteBehavior.NoAction);

                //b.HasOne<Service>()
                // .WithMany()
                // .HasForeignKey(x => x.productId)
                // .IsRequired(false)
                // .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<Order>(b =>
            {
                b.ConfigureCalculableProductsAggregateRoot
                (
                    "Orders",
                    builder.Entity<OrderDiscount>(),
                    "OrderDiscounts",
                    builder.Entity<OrderLine>(),
                    "OrderLines",
                    builder.Entity<OrderLineDiscount>(),
                    "OrderLineDiscounts"
                );

                b.Property(x => x.Number)
                 .IsRequired()
                 .HasMaxLength(OrderConsts.MaxNumberLength);
                b.Property(x => x.Type).IsRequired();
                b.Property(x => x.Statu).IsRequired();
                b.Property(x => x.Date).IsRequired();

                b.HasMany(x => x.Lines)
                 .WithOne()
                 .HasForeignKey(l => l.OrderId)
                 .IsRequired();

                b.HasOne<Client>()
                 .WithMany()
                 .HasForeignKey(x => x.ClientId)
                 .IsRequired(false)
                 .OnDelete(DeleteBehavior.NoAction);

                b.HasIndex(x => new
                {
                    x.Type,
                    x.Number
                }).IsUnique(true);

                b.HasIndex(x => x.Date);
            });

            builder.Entity<Currency>(b =>
            {
                b.ToTable(SalerConsts.DbTablePrefix + "Currencies", SalerConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.Code)
                 .IsRequired()
                 .HasMaxLength(CurrencyConsts.MaxCodeLength);
                b.Property(x => x.Name)
                 .HasMaxLength(CurrencyConsts.MaxNameLength);
                b.Property(x => x.Symbol)
                 .HasMaxLength(CurrencyConsts.MaxSymbolLength);

                b.HasIndex(x => x.Code)
                 .IsUnique(true);
            });

            builder.Entity<CurrencyDailyExchange>(b =>
            {
                b.ToTable(SalerConsts.DbTablePrefix + "CurrencyDailyExchanges", SalerConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.Date)
                 .HasColumnType("date");
                b.Property(x => x.Rate1)
                 .HasColumnType("float");
                b.Property(x => x.Rate2)
                 .HasColumnType("float");
                b.Property(x => x.Rate3)
                 .HasColumnType("float");
                b.Property(x => x.Rate4)
                 .HasColumnType("float");

                b.HasOne<Currency>()
                 .WithMany()
                 .HasForeignKey(x => x.CurrencyId)
                 .OnDelete(DeleteBehavior.Cascade)
                 .IsRequired();

                b.HasIndex(x => new
                {
                    x.Date,
                    x.CurrencyId
                }).IsUnique(true);
            });

            builder.Entity<UnitPrice>(b =>
            {
                b.ToTable(SalerConsts.DbTablePrefix + "UnitPrices", SalerConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.Code)
                 .IsRequired()
                 .HasMaxLength(UnitPriceConsts.MaxCodeLength);
                b.Property(x => x.Type).IsRequired();
                b.Property(x => x.ProductId).IsRequired();
                b.Property(x => x.UnitId).IsRequired();
                b.Property(x => x.IsVatIncluded).IsRequired();
                b.Property(x => x.PurchasePrice)
                 .HasColumnType("float")
                 .IsRequired();
                b.Property(x => x.SalesPrice)
                 .HasColumnType("float")
                 .IsRequired();
                b.Property(x => x.BeginDate).IsRequired();
                b.Property(x => x.EndDate).IsRequired();

                b.HasOne<Unit>()
                 .WithMany()
                 .HasForeignKey(x => x.UnitId)
                 .IsRequired()
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasOne<Currency>()
                 .WithMany()
                 .HasForeignKey(x => x.CurrencyId)
                 .OnDelete(DeleteBehavior.NoAction);

                b.HasOne<Client>()
                 .WithMany()
                 .HasForeignKey(x => x.ClientId)
                 .OnDelete(DeleteBehavior.NoAction);

                b.HasIndex(x => new
                {
                    x.Type,
                    x.ProductId,
                    x.BeginDate,
                    x.EndDate
                }).IsUnique(false);

                b.HasIndex(x => new
                {
                    x.Type,
                    x.Code
                }).IsUnique(true);
            });
        }
    }
}
