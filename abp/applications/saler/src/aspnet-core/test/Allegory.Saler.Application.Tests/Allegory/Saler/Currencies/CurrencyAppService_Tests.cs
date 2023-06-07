using Allegory.Saler.UnitPrices;
using Shouldly;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Xunit;

namespace Allegory.Saler.Currencies;

public class CurrencyAppService_Tests : SalerApplicationTestBase
{
    protected ICurrencyAppService CurrencyAppService { get; }

    public CurrencyAppService_Tests()
    {
        CurrencyAppService = GetRequiredService<ICurrencyAppService>();
    }

    [Fact]
    public async Task Should_Get_List_Of_Currencies()
    {
        //Act
        var result = await CurrencyAppService.ListAsync(new FilteredPagedAndSortedResultRequestDto());

        //Assert
        result.TotalCount.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task Should_Create_A_Valid_Currency()
    {
        var result = await CurrencyAppService.CreateAsync(new CurrencyCreateUpdateDto()
        {
            Code = "Kod-1",
            Name = "kod 1 açıklama",
            Symbol = "₺"
        });

        result.Id.ShouldNotBe(0);
        result.Code.ShouldBe("Kod-1");
        result.Name.ShouldBe("kod 1 açıklama");
        result.Symbol.ShouldBe("₺");
    }

    [Fact]
    public async Task Should_Update_A_Valid_Currency()
    {
        var result = await CurrencyAppService.UpdateAsync(
            1,
            new CurrencyCreateUpdateDto()
            {
                Code = "Kod-1",
                Name = "kod 1 açıklama",
                Symbol = "₺"
            });

        result.Id.ShouldBe(1);
        result.Code.ShouldBe("Kod-1");
        result.Name.ShouldBe("kod 1 açıklama");
        result.Symbol.ShouldBe("₺");
    }

    [Fact]
    public async Task Should_Delete_Currency()
    {
        await CurrencyAppService.DeleteAsync(1);
    }

    [Fact]
    public async Task Should_Delete_Currency_With_Unit_Prices()
    {
        var currency = await CurrencyAppService.CreateAsync(new CurrencyCreateUpdateDto()
        {
            Code = "Kod-1",
            Name = "kod 1 açıklama",
            Symbol = "₺"
        });

        var unitPriceAppService = GetRequiredService<IUnitPriceAppService>();
        await unitPriceAppService.CreateAsync(new UnitPriceCreateDto
        {
            Code = "Kod-1",
            Type = UnitPriceType.Item,
            ProductCode = "Malzeme-1",
            UnitCode = "Alt birim-2",
            IsVatIncluded = true,
            BeginDate = DateTime.Now.Date.AddDays(-180),
            EndDate = DateTime.Now.Date.AddDays(180),
            PurchasePrice = 100,
            SalesPrice = 150,
            CurrencyCode = "Kod-1"
        });
        await unitPriceAppService.CreateAsync(new UnitPriceCreateDto
        {
            Code = "Kod-2",
            Type = UnitPriceType.Item,
            ProductCode = "Malzeme-1",
            UnitCode = "Alt birim-2",
            IsVatIncluded = true,
            BeginDate = DateTime.Now.Date.AddDays(-180),
            EndDate = DateTime.Now.Date.AddDays(180),
            PurchasePrice = 100,
            SalesPrice = 150,
            CurrencyCode = "Kod-1"
        });

        await CurrencyAppService.DeleteAsync(currency.Id);
    }
}
