using Allegory.Saler.UnitPrices;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Validation;
using Xunit;

namespace Allegory.Saler.Services;

public class ServiceAppService_Tests : SalerApplicationTestBase
{
    protected IServiceAppService ServiceAppService { get; }

    public ServiceAppService_Tests()
    {
        ServiceAppService = GetRequiredService<IServiceAppService>();
    }

    [Fact]
    public async Task Should_Get_List_Of_Services()
    {
        //Act
        var result = await ServiceAppService.ListAsync(new FilteredPagedAndSortedResultRequestDto());

        //Assert
        result.TotalCount.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task Should_Get_List_Of_Service_Lookup()
    {
        //Act
        var result = await ServiceAppService.ListServiceLookupAsync(new GetServiceLookupListDto()
        {
            Date = DateTime.Now
        });

        //Assert
        result.TotalCount.ShouldBeGreaterThan(0);
        result.Items[9].Price.Value.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task Should_Get_List_Of_Service_Lookup_Price_By_Specified_Client()
    {
        var unitPriceAppService = GetRequiredService<IUnitPriceAppService>();
        await unitPriceAppService.CreateAsync(new UnitPriceCreateDto
        {
            Code = "Client-Bf",
            Type = UnitPriceType.Service,
            ProductCode = "Hizmet-1",
            UnitCode = "Alt birim-1",
            BeginDate = DateTime.Now.Date.AddDays(-1),
            EndDate = DateTime.Now.Date.AddDays(1),
            ClientCode = "Müşteri-1",
            SalesPrice = 1
        });

        //Act
        var result = await ServiceAppService.ListServiceLookupAsync(new GetServiceLookupListDto()
        {
            Date = DateTime.Now,
            ClientCode = "Müşteri-1",
            IsSales = true
        });

        var result2 = await ServiceAppService.ListServiceLookupAsync(new GetServiceLookupListDto()
        {
            Date = DateTime.Now,
            IsSales = true
        });

        //Assert
        result.Items.FirstOrDefault(x => x.Code == "Hizmet-1").Price.Value.ShouldBe(1);
        result2.Items.FirstOrDefault(x => x.Code == "Hizmet-1").Price.Value.ShouldBe(2);
    }

    [Fact]
    public async Task Should_Create_A_Valid_Service()
    {
        var result = await ServiceAppService.CreateAsync(new ServiceCreateDto()
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
             await ServiceAppService.CreateAsync(new ServiceCreateDto()
             {
                 Code = "Kod-1",
                 SalesVatRate = 101,
                 UnitGroupCode = "Ana birim-1"
             });
         });

        exception.ValidationErrors.First().MemberNames.First().ShouldBe("SalesVatRate");
    }

    [Fact]
    public async Task Should_Update_A_Valid_Service()
    {
        var result = await ServiceAppService.UpdateAsync(
            1,
            new ServiceUpdateDto()
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
}
