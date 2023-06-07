using Allegory.Saler.Orders;
using Allegory.Saler.UnitPrices;
using Allegory.Saler.Units;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Validation;
using Xunit;

namespace Allegory.Saler.Items;

public class ItemAppService_Tests : SalerApplicationTestBase
{
    protected IItemAppService ItemAppService { get; }

    public ItemAppService_Tests()
    {
        ItemAppService = GetRequiredService<IItemAppService>();
    }

    [Fact]
    public async Task Should_Get_List_Of_Items()
    {
        //Act
        var result = await ItemAppService.ListAsync(new FilteredPagedAndSortedResultRequestDto());

        //Assert
        result.TotalCount.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task Should_Get_List_Of_Item_Lookup()
    {
        //Act
        var result = await ItemAppService.ListItemLookupAsync(new GetItemLookupListDto()
        {
            Date = DateTime.Now
        });

        //Assert
        result.TotalCount.ShouldBeGreaterThan(0);
        result.Items[9].Price.Value.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task Should_Get_List_Of_Item_Lookup_Price_By_Specified_Client()
    {
        var unitPriceAppService = GetRequiredService<IUnitPriceAppService>();
        await unitPriceAppService.CreateAsync(new UnitPriceCreateDto
        {
            Code = "Client-Bf",
            Type = UnitPriceType.Item,
            ProductCode = "Malzeme-1",
            UnitCode = "Alt birim-1",
            BeginDate = DateTime.Now.Date.AddDays(-1),
            EndDate = DateTime.Now.Date.AddDays(1),
            ClientCode = "Müşteri-1",
            SalesPrice = 1
        });

        //Act
        var result = await ItemAppService.ListItemLookupAsync(new GetItemLookupListDto()
        {
            Date = DateTime.Now,
            ClientCode = "Müşteri-1",
            IsSales = true
        });

        var result2 = await ItemAppService.ListItemLookupAsync(new GetItemLookupListDto()
        {
            Date = DateTime.Now,
            IsSales = true
        });

        //Assert
        result.Items.FirstOrDefault(x => x.Code == "Malzeme-1").Price.Value.ShouldBe(1);
        result2.Items.FirstOrDefault(x => x.Code == "Malzeme-1").Price.Value.ShouldBe(2);
    }

    [Fact]
    public async Task Should_Create_A_Valid_Item()
    {
        var result = await ItemAppService.CreateAsync(new ItemCreateDto()
        {
            Code = "Kod-1",
            Name = "kod 1 açıklama",
            Type = ItemType.Item,
            SalesVatRate = 20,
            PurchaseVatRate = 10,
            UnitGroupCode = "Ana birim-1",
            DeductionCode = "601",
            SalesDeductionPart1 = 1,
            SalesDeductionPart2 = 2
        });

        result.Id.ShouldNotBe(0);
        result.Code.ShouldBe("Kod-1");
        result.Name.ShouldBe("kod 1 açıklama");
        result.SalesVatRate.ShouldBe((byte)20);
        result.PurchaseVatRate.ShouldBe((byte)10);
    }

    [Fact]
    public async Task Should_Not_Create_When_Vat_Rates_Less_Than_Zero_Or_Greater_Than_One_Hundred()
    {
        var exception = await Assert.ThrowsAsync<AbpValidationException>(async () =>
        {
            await ItemAppService.CreateAsync(new ItemCreateDto()
            {
                Code = "Kod-1",
                SalesVatRate = 101,
                UnitGroupCode = "Ana birim-1"
            });
        });

        exception.ValidationErrors.First().MemberNames.First().ShouldBe("SalesVatRate");
    }

    [Fact]
    public async Task Should_Update_A_Valid_Item()
    {
        var result = await ItemAppService.UpdateAsync(
            1,
            new ItemUpdateDto()
            {
                Code = "Kod-1",
                Name = "kod 1 açıklama",
                SalesVatRate = 20,
                PurchaseVatRate = 10,
                UnitGroupCode = "Ana birim-1",
                DeductionCode = "601",
                SalesDeductionPart1 = 1,
                SalesDeductionPart2 = 2
            });

        result.Code.ShouldBe("Kod-1");
        result.Name.ShouldBe("kod 1 açıklama");
        result.SalesVatRate.ShouldBe((byte)20);
        result.PurchaseVatRate.ShouldBe((byte)10);
    }

    [Fact]
    public async Task Should_Not_Update_Unit_Group_When_Item_Has_Transaction_Records()
    {
        var exception = await Assert.ThrowsAsync<ThereIsTransactionRecordException>(async () =>
        {
            var result = await ItemAppService.UpdateAsync(
                1,
                new ItemUpdateDto()
                {
                    Code = "Malzeme-1",
                    Name = "Malzeme 1 açıklama",
                    SalesVatRate = 20,
                    PurchaseVatRate = 10,
                    UnitGroupCode = "Ana birim-3"
                });
        });

        var exception2 = await Assert.ThrowsAsync<ThereIsTransactionRecordException>(async () =>
          {
              var result = await ItemAppService.UpdateAsync(
                  2,
                  new ItemUpdateDto()
                  {
                      Code = "Malzeme-2",
                      Name = "Malzeme 2 açıklama",
                      SalesVatRate = 20,
                      PurchaseVatRate = 10,
                      UnitGroupCode = "Ana birim-3"
                  });
          });

        exception.EntityType.ShouldBe(typeof(UnitGroup));
        exception.TransactionEntityType.ShouldBe(typeof(UnitPrice));
        exception2.EntityType.ShouldBe(typeof(UnitGroup));
        exception2.TransactionEntityType.ShouldBe(typeof(Order));
    }
}
