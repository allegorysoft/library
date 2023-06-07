using Allegory.Saler.UnitPrices;
using Shouldly;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Xunit;

namespace Allegory.Saler.Clients;

public class ClientAppService_Tests : SalerApplicationTestBase
{
    protected IClientAppService ClientAppService { get; }

    public ClientAppService_Tests()
    {
        ClientAppService = GetRequiredService<IClientAppService>();
    }

    [Fact]
    public async Task Should_Get_List_Of_Clients()
    {
        //Act
        var result = await ClientAppService.ListAsync(new FilteredPagedAndSortedResultRequestDto());

        //Assert
        result.TotalCount.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task Should_Create_A_Valid_Client()
    {
        var result = await ClientAppService.CreateAsync(new ClientCreateDto()
        {
            Code = "Kod-1",
            Title = "kod 1 açıklama",
        });

        result.Id.ShouldNotBe(0);
        result.Code.ShouldBe("Kod-1");
        result.Title.ShouldBe("kod 1 açıklama");
    }

    [Fact]
    public async Task Should_Update_A_Valid_Client()
    {
        var result = await ClientAppService.UpdateAsync(
            1,
            new ClientUpdateDto()
            {
                Code = "Kod-1 güncellendi",
                Title = "kod 1 güncellendi",
                IdentityNumber = "identity"
            });

        result.Id.ShouldBe(1);
        result.Code.ShouldBe("Kod-1 güncellendi");
        result.Title.ShouldBe("kod 1 güncellendi");
        result.IdentityNumber.ShouldBe("identity");
    }

    [Fact]
    public async Task Should_Delete_Client()
    {
        await ClientAppService.DeleteAsync(1);
    }

    [Fact]
    public async Task Should_Delete_Client_With_Unit_Prices()
    {
        var client = await ClientAppService.CreateAsync(new ClientCreateDto()
        {
            Code = "Kod-1",
            Title = "kod 1 açıklama",
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
            ClientCode = "Kod-1"
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
            ClientCode = "Kod-1"
        });

        await ClientAppService.DeleteAsync(client.Id);

        var exception = await Assert.ThrowsAsync<CodeNotFoundException>(
            async () => await unitPriceAppService.GetByCodeAsync("Kod-1", UnitPriceType.Item));

        var exception2 = await Assert.ThrowsAsync<CodeNotFoundException>(
            async () => await unitPriceAppService.GetByCodeAsync("Kod-2", UnitPriceType.Item));

        exception.EntityCode.ShouldBe("Kod-1");
        exception2.EntityCode.ShouldBe("Kod-2");
    }

    [Fact]
    public async Task Should_Not_Create_Client_When_Type_is_Indivudial_And_Name_Or_Surname_Is_Null()
    {
        var exception = await Assert.ThrowsAsync<BusinessException>(async () =>
        await ClientAppService.CreateAsync(new ClientCreateDto()
        {
            Code = "Kod-1",
            Title = "kod 1 açıklama",
            Type = ClientType.Individual,
        }));

        exception.Code.ShouldBe(SalerDomainErrorCodes.ClientNameAndSurnameMustSet);
    }
}
