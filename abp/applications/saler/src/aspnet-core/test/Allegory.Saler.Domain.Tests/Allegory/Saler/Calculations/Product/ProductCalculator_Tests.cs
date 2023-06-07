using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Xunit;

namespace Allegory.Saler.Calculations.Product;

public class ProductCalculator_Tests : SalerDomainTestBase
{
    ProductCalculator<Discount, CalculableProduct<Discount>, Discount> ProductCalculator { get; }

    public ProductCalculator_Tests()
    {
        ProductCalculator = GetRequiredService<ProductCalculator<Discount, CalculableProduct<Discount>, Discount>>();
    }


    [Fact]
    public void Should_Calculate_Total()
    {
        var result = ProductCalculator.Calculate(new CalculableProduct<Discount>(
            quantity: 1,
            price: 10,
            vatRate: 18,
            isVatIncluded: true));

        result.Total.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Should_Calculate_Vat()
    {
        var result = ProductCalculator.Calculate(new CalculableProduct<Discount>(
            quantity: 1,
            price: 10,
            vatRate: 18,
            isVatIncluded: false));

        result.VatBase.ShouldBe(10);
        result.VatAmount.ShouldBe(1.8m);
    }

    [Fact]
    public void Should_Not_Calculate_Vat_When_Vat_Rate_Less_Than_Zero_Or_Greater_Than_One_Hundred()
    {
        var exception = Assert.Throws<BusinessException>(() =>
        {
            ProductCalculator.Calculate(new CalculableProduct<Discount>(
                quantity: 1,
                price: 10,
                vatRate: 101,
                isVatIncluded: false));
        });

        var exception2 = Assert.Throws<BusinessException>(() =>
        {
            ProductCalculator.Calculate(new CalculableProduct<Discount>(
                quantity: 1,
                price: 10,
                vatRate: -1,
                isVatIncluded: false));
        });

        exception.Code.ShouldBe(SalerDomainErrorCodes.VatRateMustBeBetweenZeroAndOneHundred);
        exception2.Code.ShouldBe(SalerDomainErrorCodes.VatRateMustBeBetweenZeroAndOneHundred);
    }

    [Fact]
    public void Should_Calculate_Vat_Exclude_Deduction()
    {
        var result = ProductCalculator.Calculate(new CalculableProduct<Discount>(
            quantity: 1,
            price: 100,
            vatRate: 18,
            isVatIncluded: false,
            deductionPart1: 1,
            deductionPart2: 2,
            deductionCode: "610"));

        result.VatBase.ShouldBe(100);
        result.VatAmount.ShouldBe(9);
    }

    [Fact]
    public void Should_Calculate_Price_By_Total()
    {
        var result = ProductCalculator.Calculate(new CalculableProduct<Discount>(
            quantity: 2,
            price: 0,
            vatRate: 18,
            isVatIncluded: true,
            total: 10));

        result.Price.ShouldBe(5);
    }

    [Fact]
    public void Should_Calculate_Quantity_By_Total()
    {
        var result = ProductCalculator.Calculate(new CalculableProduct<Discount>(
            quantity: 0,
            price: 5,
            vatRate: 18,
            isVatIncluded: true,
            total: 10));


        result.Quantity.ShouldBe(2);
    }

    [Fact]
    public void Should_Calculate_Discounts_By_Rate_With_Vat_Included()
    {
        var entity = new CalculableProduct<Discount>(
            quantity: 1,
            price: 100,
            vatRate: 18,
            isVatIncluded: true);
        entity.Discounts.Add(new Discount(rate: 10, total: 0));
        entity.Discounts.Add(new Discount(rate: 50, total: 0));

        var result = ProductCalculator.Calculate(entity);

        result.Total.ShouldBe(100);
        result.CalculatedTotal.ShouldBe(45);
        result.DiscountTotal.ShouldBe(55);
        result.VatBase.ShouldBe(38.14m);
        result.VatAmount.ShouldBe(6.86m);
    }

    [Fact]
    public void Should_Calculate_Discounts_By_Rate_With_Vat_Excluded()
    {
        var entity = new CalculableProduct<Discount>(
            quantity: 1,
            price: 100,
            vatRate: 18,
            isVatIncluded: false);
        entity.Discounts.Add(new Discount(rate: 10, total: 0));
        entity.Discounts.Add(new Discount(rate: 50, total: 0));

        var result = ProductCalculator.Calculate(entity);

        result.Total.ShouldBe(100);
        result.CalculatedTotal.ShouldBe(45);
        result.DiscountTotal.ShouldBe(55);
        result.VatBase.ShouldBe(45);
        result.VatAmount.ShouldBe(8.10m);
    }

    [Fact]
    public void Should_Not_Calculate_Discounts_When_Rate_Less_Than_Zero_Or_Greater_Than_One_Hundred()
    {
        var exception = Assert.Throws<BusinessException>(() =>
        {
            var discount = new Discount(rate: -1, total: 0);
        });

        var exception2 = Assert.Throws<BusinessException>(() =>
        {
            var discount = new Discount(rate: 101, total: 0);
        });

        exception.Code.ShouldBe(SalerDomainErrorCodes.DiscountMustBeBetweenZeroAndTotal);
        exception2.Code.ShouldBe(SalerDomainErrorCodes.DiscountMustBeBetweenZeroAndTotal);
    }

    [Fact]
    public void Should_Not_Calculate_Discounts_When_Total_Less_Than_Zero_Or_Greater_Than_Line_Total()
    {
        var exception = Assert.Throws<BusinessException>(() =>
        {
            var entity = new CalculableProduct<Discount>(
                quantity: 1,
                price: 100,
                vatRate: 18,
                isVatIncluded: true);
            entity.Discounts.Add(new Discount(rate: 0, total: 101));

            ProductCalculator.Calculate(entity);
        });

        var exception2 = Assert.Throws<BusinessException>(() =>
        {
            var discount = new Discount(rate: 0, total: -1);
        });

        exception.Code.ShouldBe(SalerDomainErrorCodes.DiscountMustBeBetweenZeroAndTotal);
        exception2.Code.ShouldBe(SalerDomainErrorCodes.DiscountMustBeBetweenZeroAndTotal);
    }

    [Fact]
    public void Should_Calculate_Aggregate_Root()
    {
        #region Arrange
        var entity = new CalculableProductsAggregateRoot<Discount, CalculableProduct<Discount>, Discount>();

        var entityLine1 = new CalculableProduct<Discount>(1, 1000, 18, false);
        var entityLine2 = new CalculableProduct<Discount>(1, 1100, 18, false);
        entityLine2.Discounts.Add(new Discount(0, 100));
        var entityLine3 = new CalculableProduct<Discount>(1, 1180, 18, true);
        entity.Lines.Add(entityLine1);
        entity.Lines.Add(entityLine2);
        entity.Lines.Add(entityLine3);

        foreach (var entityLine in entity.Lines)
            ProductCalculator.SetCalculate(entityLine);
        #endregion

        ProductCalculator.CalculateAggregateRoot(
            entity,
            new List<Discount>
            {
                new Discount(0, 1000)
            });

        entity.TotalDiscount.ShouldBe(1100);
        entity.TotalVatBase.ShouldBe(2000);
        entity.TotalVatAmount.ShouldBe(360);
        entity.TotalGross.ShouldBe(2360);
        entity.Discounts[0].Total.ShouldBe(1000);
    }

    [Fact]
    public void Should_Calculate_From_Currency_Price()
    {
        var entity = new CalculableProduct<Discount>();
        entity.SetQuantity(1);
        entity.CurrencyId = 1;
        entity.SetCurrencyPrice(10);
        entity.SetVatRate(18);
        entity.SetCurrencyRate(10.1234m);

        var result = ProductCalculator.Calculate(entity);

        result.Price.ShouldBe(101.23m, 0.009m);
        result.CalculatedTotal.ShouldBe(101.23m);
        result.VatAmount.ShouldBe(18.22m);
    }

    [Fact]
    public void Should_Calculate_From_Currency_Total()
    {
        var entity = new CalculableProduct<Discount>();
        entity.SetQuantity(2);
        entity.CurrencyId = 1;
        entity.SetCurrencyTotal(20);
        entity.SetVatRate(18);
        entity.SetCurrencyRate(10.1234m);

        var result = ProductCalculator.Calculate(entity);

        result.Price.ShouldBe(101.23m, 0.009m);
        result.CalculatedTotal.ShouldBe(202.47m);
        result.VatAmount.ShouldBe(36.44m);
    }

    [Fact]
    public void Should_Calculate_Currency()
    {
        var entity = new CalculableProduct<Discount>();
        entity.SetQuantity(2);
        entity.CurrencyId = 1;
        entity.SetVatRate(18);
        entity.SetPrice(10);
        entity.SetCurrencyRate(10);

        var result = ProductCalculator.Calculate(entity);

        result.CurrencyPrice.ShouldBe(1);
        result.CurrencyTotal.ShouldBe(2);
        result.VatAmount.ShouldBe(3.6m);
    }
}
