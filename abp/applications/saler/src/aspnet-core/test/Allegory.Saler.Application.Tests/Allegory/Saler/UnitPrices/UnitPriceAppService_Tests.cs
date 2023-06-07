using Allegory.Saler.Clients;
using Allegory.Saler.Currencies;
using Allegory.Saler.Items;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Validation;
using Xunit;

namespace Allegory.Saler.UnitPrices;

public class UnitPriceAppService_Tests : SalerApplicationTestBase
{
    protected IUnitPriceAppService UnitPriceAppService { get; }

    public UnitPriceAppService_Tests()
    {
        UnitPriceAppService = GetRequiredService<IUnitPriceAppService>();
    }

    [Fact]
    public async Task Should_Get_List_Of_Unit_Prices()
    {
        //Act
        var result = await UnitPriceAppService.ListAsync(new FilteredPagedAndSortedResultRequestDto());

        //Assert
        result.TotalCount.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task Should_Create_A_Valid_Unit_Price()
    {
        var result = await UnitPriceAppService.CreateAsync(new UnitPriceCreateDto()
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
        });

        result.Id.ShouldNotBe(0);
        result.Code.ShouldBe("Kod-1");
        result.PurchasePrice.ShouldBe(100);
        result.SalesPrice.ShouldBe(150);
        result.IsVatIncluded.ShouldBe(true);
    }

    [Fact]
    public async Task Should_Not_Create_When_Item_Not_Found()
    {
        var exception = await Assert.ThrowsAsync<CodeNotFoundException>(async () =>
         {
             await UnitPriceAppService.CreateAsync(new UnitPriceCreateDto()
             {
                 Code = "Kod-1",
                 Type = UnitPriceType.Item,
                 ProductCode = "Olmayan-Malzeme",
                 UnitCode = "alt birim",
             });
         });

        exception.EntityCode.ShouldBe("Olmayan-Malzeme");
        exception.EntityType.ShouldBe(typeof(Item));
    }

    [Fact]
    public async Task Should_Not_Create_When_Price_Less_Than_Zero()
    {
        var exception = await Assert.ThrowsAsync<AbpValidationException>(async () =>
        {
            await UnitPriceAppService.CreateAsync(new UnitPriceCreateDto()
            {
                Code = "Kod-1",
                Type = UnitPriceType.Item,
                ProductCode = "Olmayan-Malzeme",
                UnitCode = "alt birim",
                SalesPrice = -1
            });
        });

        var exception2 = await Assert.ThrowsAsync<AbpValidationException>(async () =>
        {
            await UnitPriceAppService.CreateAsync(new UnitPriceCreateDto()
            {
                Code = "Kod-1",
                Type = UnitPriceType.Item,
                ProductCode = "Olmayan-Malzeme",
                UnitCode = "alt birim",
                PurchasePrice = -1
            });
        });

        exception.ValidationErrors.First().MemberNames.First().ShouldBe("SalesPrice");
        exception2.ValidationErrors.First().MemberNames.First().ShouldBe("PurchasePrice");
    }

    [Fact]
    public async Task Should_Not_Create_When_Currency_Not_Found()
    {
        var exception = await Assert.ThrowsAsync<CodeNotFoundException>(async () =>
        {
            await UnitPriceAppService.CreateAsync(new UnitPriceCreateDto()
            {
                Code = "Kod-1",
                Type = UnitPriceType.Item,
                ProductCode = "Malzeme-1",
                UnitCode = "Alt birim-1",
                CurrencyCode = "Olmayan-döviz",
            });
        });

        exception.EntityCode.ShouldBe("Olmayan-döviz");
        exception.EntityType.ShouldBe(typeof(Currency));
    }

    [Fact]
    public async Task Should_Not_Create_When_Client_Not_Found()
    {
        var exception = await Assert.ThrowsAsync<CodeNotFoundException>(async () =>
        {
            await UnitPriceAppService.CreateAsync(new UnitPriceCreateDto()
            {
                Code = "Kod-1",
                Type = UnitPriceType.Item,
                ProductCode = "Malzeme-1",
                UnitCode = "Alt birim-1",
                ClientCode = "Olmayan-Musteri",
            });
        });

        exception.EntityCode.ShouldBe("Olmayan-Musteri");
        exception.EntityType.ShouldBe(typeof(Client));
    }

    [Fact]
    public async Task Should_Update_A_Valid_Unit_Price()
    {
        var result = await UnitPriceAppService.UpdateAsync(
            1,
            new UnitPriceUpdateDto()
            {
                Code = "Kod-2",
                ProductCode = "Malzeme-1",
                UnitCode = "Alt birim-3",
                IsVatIncluded = true,
                BeginDate = DateTime.Now.Date.AddDays(-180),
                EndDate = DateTime.Now.Date.AddDays(180),
                PurchasePrice = 100,
                SalesPrice = 150,
            });

        result.Code.ShouldBe("Kod-2");
        result.PurchasePrice.ShouldBe(100);
        result.SalesPrice.ShouldBe(150);
        result.IsVatIncluded.ShouldBe(true);
        result.BeginDate.ShouldBe(DateTime.Now.Date.AddDays(-180));
    }

    [Fact]
    public async Task Should_Get_Price()
    {
        var price = await UnitPriceAppService.GetPriceAsync(
            "Malzeme-1",
            UnitPriceType.Item,
            "Alt birim-1",
            DateTime.Now,
            true);

        price.ShouldBeGreaterThan(0);
    }
}
