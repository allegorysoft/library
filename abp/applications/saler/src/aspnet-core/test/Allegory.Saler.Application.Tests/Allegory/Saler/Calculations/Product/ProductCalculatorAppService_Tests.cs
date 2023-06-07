using Shouldly;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Xunit;

namespace Allegory.Saler.Calculations.Product;

public class ProductCalculatorAppService_Tests : SalerApplicationTestBase
{
    protected IProductCalculatorAppService ProductCalculatorAppService { get; }

    public ProductCalculatorAppService_Tests()
    {
        ProductCalculatorAppService = GetRequiredService<IProductCalculatorAppService>();
    }

    [Fact]
    public async Task Should_Calculate_Total()
    {
        var result = await ProductCalculatorAppService.CalculateAsync(new CalculableProductInputDto
        {
            Quantity = 1,
            Price = 10,
            VatRate = 18,
            IsVatIncluded = true
        });

        result.Total.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task Should_Calculate_Vat()
    {
        var result = await ProductCalculatorAppService.CalculateAsync(new CalculableProductInputDto
        {
            Quantity = 1,
            Price = 10,
            VatRate = 18,
            IsVatIncluded = false
        });

        result.VatBase.ShouldBe(10);
        result.VatAmount.ShouldBe(1.8m);
    }

    [Fact]
    public async Task Should_Not_Calculate_Vat_When_Vat_Rate_Less_Than_Zero_Or_Greater_Than_One_Hundred()
    {
        var exception = await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await ProductCalculatorAppService.CalculateAsync(new CalculableProductInputDto
            {
                Quantity = 1,
                Price = 10,
                VatRate = 101,
                IsVatIncluded = false
            });
        });

        var exception2 = await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await ProductCalculatorAppService.CalculateAsync(new CalculableProductInputDto
            {
                Quantity = 1,
                Price = 10,
                VatRate = -1,
                IsVatIncluded = false
            });
        });

        exception.Code.ShouldBe(SalerDomainErrorCodes.VatRateMustBeBetweenZeroAndOneHundred);
        exception2.Code.ShouldBe(SalerDomainErrorCodes.VatRateMustBeBetweenZeroAndOneHundred);
    }

    [Fact]
    public async Task Should_Calculate_Vat_Exclude_Deduction()
    {
        var result = await ProductCalculatorAppService.CalculateAsync(new CalculableProductInputDto
        {
            Quantity = 1,
            Price = 100,
            VatRate = 18,
            IsVatIncluded = false,
            DeductionPart1 = 1,
            DeductionPart2 = 2,
            DeductionCode = "610"
        });

        result.VatBase.ShouldBe(100);
        result.VatAmount.ShouldBe(9);
    }

    [Fact]
    public async Task Should_Calculate_Price_By_Total()
    {
        var result = await ProductCalculatorAppService.CalculateAsync(new CalculableProductInputDto
        {
            Quantity = 2,
            VatRate = 18,
            IsVatIncluded = true,
            Total = 10
        });

        result.Price.ShouldBe(5);
    }

    [Fact]
    public async Task Should_Calculate_Quantity_By_Total()
    {
        var result = await ProductCalculatorAppService.CalculateAsync(new CalculableProductInputDto
        {
            Price = 5,
            VatRate = 18,
            IsVatIncluded = true,
            Total = 10
        });

        result.Quantity.ShouldBe(2);
    }

    [Fact]
    public async Task Should_Calculate_Discounts_By_Rate_With_Vat_Included()
    {
        var entity = new CalculableProductInputDto
        {
            Quantity = 1,
            Price = 100,
            VatRate = 18,
            IsVatIncluded = true,
            Discounts = new Collection<DiscountDto>
                {
                    new DiscountDto()
                    {
                        Rate = 10
                    },
                    new DiscountDto()
                    {
                        Rate = 50
                    }
                }
        };

        var result = await ProductCalculatorAppService.CalculateAsync(entity);

        result.Total.ShouldBe(100);
        result.CalculatedTotal.ShouldBe(45);
        result.DiscountTotal.ShouldBe(55);
        result.VatBase.ShouldBe(38.14m);
        result.VatAmount.ShouldBe(6.86m);
    }

    [Fact]
    public async Task Should_Calculate_Discounts_By_Rate_With_Vat_Excluded()
    {
        var entity = new CalculableProductInputDto
        {
            Quantity = 1,
            Price = 100,
            VatRate = 18,
            IsVatIncluded = false,
            Discounts = new Collection<DiscountDto>
                {
                    new DiscountDto()
                    {
                        Rate = 10
                    },
                    new DiscountDto()
                    {
                        Rate = 50
                    }
                }
        };

        var result = await ProductCalculatorAppService.CalculateAsync(entity);

        result.Total.ShouldBe(100);
        result.CalculatedTotal.ShouldBe(45);
        result.DiscountTotal.ShouldBe(55);
        result.VatBase.ShouldBe(45);
        result.VatAmount.ShouldBe(8.10m);
    }

    [Fact]
    public async Task Should_Not_Calculate_Discounts_When_Rate_Less_Than_Zero_Or_Greater_Than_One_Hundred()
    {
        var exception = await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await ProductCalculatorAppService.CalculateAsync(new CalculableProductInputDto
            {
                Quantity = 1,
                Price = 100,
                VatRate = 18,
                IsVatIncluded = true,
                Discounts = new Collection<DiscountDto>
                {
                    new DiscountDto()
                    {
                        Rate = -1
                    }
                }
            });
        });

        var exception2 = await Assert.ThrowsAsync<BusinessException>(async () =>
         {
             await ProductCalculatorAppService.CalculateAsync(new CalculableProductInputDto
             {
                 Quantity = 1,
                 Price = 100,
                 VatRate = 18,
                 IsVatIncluded = true,
                 Discounts = new Collection<DiscountDto>
                 {
                    new DiscountDto()
                    {
                        Rate = 101
                    }
                 }
             });
         });

        exception.Code.ShouldBe(SalerDomainErrorCodes.DiscountMustBeBetweenZeroAndTotal);
        exception2.Code.ShouldBe(SalerDomainErrorCodes.DiscountMustBeBetweenZeroAndTotal);
    }

    [Fact]
    public async Task Should_Not_Calculate_Discounts_When_Total_Less_Than_Zero_Or_Greater_Than_Line_Total()
    {
        var exception = await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await ProductCalculatorAppService.CalculateAsync(new CalculableProductInputDto
            {
                Quantity = 1,
                Price = 100,
                VatRate = 18,
                IsVatIncluded = true,
                Discounts = new Collection<DiscountDto>
                {
                    new DiscountDto()
                    {
                        Total = 101
                    }
                }
            });
        });

        var exception2 = await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await ProductCalculatorAppService.CalculateAsync(new CalculableProductInputDto
            {
                Quantity = 1,
                Price = 100,
                VatRate = 18,
                IsVatIncluded = false,
                Discounts = new Collection<DiscountDto>
                {
                    new DiscountDto()
                    {
                        Total = -1
                    }
                }
            });
        });

        exception.Code.ShouldBe(SalerDomainErrorCodes.DiscountMustBeBetweenZeroAndTotal);
        exception2.Code.ShouldBe(SalerDomainErrorCodes.DiscountMustBeBetweenZeroAndTotal);
    }

    [Fact]
    public async Task Should_Not_Calculate_Aggregate_Root_Currency_Has_Value_And_Rate_Is_Null()
    {
        var exception = await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await ProductCalculatorAppService.CalculateAggregateRootAsync(
                new CalculableProductAggregateRootInputDto
                {
                    CurrencyCode = "USD",
                });
        });

        var exception2 = await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await ProductCalculatorAppService.CalculateAggregateRootAsync(
                new CalculableProductAggregateRootInputDto
                {
                    Lines = new List<CalculableProductInputDto>()
                    {
                        new CalculableProductInputDto
                        {
                            CurrencyCode="USD",
                        }
                    }
                });
        });

        exception.Code.ShouldBe(SalerDomainErrorCodes.CurrencyInformationIncorrect);
        exception2.Code.ShouldBe(SalerDomainErrorCodes.CurrencyInformationIncorrect);
    }
}
