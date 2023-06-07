using Allegory.Saler.Calculations.Product;
using Allegory.Saler.Currencies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Allegory.Saler.EntityFrameworkCore;

public static class SalerEntityTypeBuilderExtensions
{
    public static void ConfigureCalculableProductsAggregateRoot<A, AD, P, D>(
        this EntityTypeBuilder<A> a,
        string calculableProductsAggregateRootTableName,
        EntityTypeBuilder<AD> ad,
        string aggregateRootDiscountTableName,
        EntityTypeBuilder<P> p,
        string calculableProductTableName,
        EntityTypeBuilder<D> d,
        string discountTableName)
        where A : CalculableProductsAggregateRoot<AD, P, D>
        where P : CalculableProduct<D>
        where AD : Discount
        where D : Discount
    {
        a.ToTable(SalerConsts.DbTablePrefix + calculableProductsAggregateRootTableName, SalerConsts.DbSchema);
        a.ConfigureByConvention();

        a.Property(x => x.TotalDiscount)
         .HasColumnType("float")
         .IsRequired();
        a.Property(x => x.TotalVatBase)
         .HasColumnType("float")
         .IsRequired();
        a.Property(x => x.TotalVatAmount)
         .HasColumnType("float")
         .IsRequired();
        a.Property(x => x.TotalGross)
         .HasColumnType("float")
         .IsRequired();

        a.Property(x => x.CurrencyRate)
         .HasColumnType("float");
        a.HasOne<Currency>()
         .WithMany()
         .HasForeignKey(x => x.CurrencyId)
         .OnDelete(DeleteBehavior.NoAction);

        p.ConfigureCalculableProduct(
            calculableProductTableName,
            d,
            discountTableName);

        a.HasMany(x => x.Discounts)
         .WithOne()
         .HasForeignKey(uc => uc.ParentId)
         .IsRequired();

        ad.ConfigureDiscount(aggregateRootDiscountTableName);
    }

    public static void ConfigureCalculableProduct<P, D>(
        this EntityTypeBuilder<P> p,
        string calculableProductTableName,
        EntityTypeBuilder<D> d,
        string discountTableName)
        where P : CalculableProduct<D>
        where D : Discount
    {
        p.ToTable(SalerConsts.DbTablePrefix + calculableProductTableName, SalerConsts.DbSchema);
        p.ConfigureByConvention();

        p.Property(x => x.Quantity)
         .HasColumnType("float")
         .IsRequired();
        p.Property(x => x.Price)
         .HasColumnType("float")
         .IsRequired();
        p.Property(x => x.VatRate)
         .HasColumnType("float")
         .IsRequired();
        p.Property(x => x.VatBase)
         .HasColumnType("float")
         .IsRequired();
        p.Property(x => x.VatAmount)
         .HasColumnType("float")
         .IsRequired();
        p.Property(x => x.IsVatIncluded).IsRequired();
        p.Property(x => x.Total)
         .HasColumnType("float")
         .IsRequired();
        p.Property(x => x.CalculatedTotal)
         .HasColumnType("float");
        p.Property(x => x.DiscountTotal)
         .HasColumnType("float");
        
        p.Ignore(x => x.CurrencyPrice)
         .Ignore(x => x.CurrencyTotal);
        p.Property(x => x.CurrencyRate)
         .HasColumnType("float");
        p.HasOne<Currency>()
         .WithMany()
         .HasForeignKey(x => x.CurrencyId)
         .OnDelete(DeleteBehavior.NoAction);

        p.HasMany(x => x.Discounts)
         .WithOne()
         .HasForeignKey(uc => uc.ParentId)
         .IsRequired();

        d.ConfigureDiscount(discountTableName);
        p.ConfigureDeduction();
    }

    public static void ConfigureDiscount<D>(
        this EntityTypeBuilder<D> p,
        string discountTableName)
    where D : Discount
    {
        p.ToTable(SalerConsts.DbTablePrefix + discountTableName, SalerConsts.DbSchema);
        p.ConfigureByConvention();

        p.Property(x => x.Rate)
         .HasColumnType("float")
         .IsRequired();
        p.Property(x => x.Total)
         .HasColumnType("float")
         .IsRequired();
    }

    public static void ConfigureDeduction<D>(this EntityTypeBuilder<D> p)
        where D : class, IDeduction
    {
        p.Property(x => x.DeductionCode)
         .HasMaxLength(DeductionConsts.MaxDeductionCodeLength);
    }
}
