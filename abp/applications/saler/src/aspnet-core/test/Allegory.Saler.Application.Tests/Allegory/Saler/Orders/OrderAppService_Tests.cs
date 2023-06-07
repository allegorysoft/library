using Allegory.Saler.Items;
using Allegory.Standart.Filter.Concrete;
using Allegory.Standart.Filter.Enums;
using Shouldly;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Validation;
using Xunit;

namespace Allegory.Saler.Orders;

public class OrderAppService_Tests : SalerApplicationTestBase
{
    protected IOrderAppService OrderAppService { get; }

    public OrderAppService_Tests()
    {
        OrderAppService = GetRequiredService<IOrderAppService>();
    }

    [Fact]
    public async Task Should_Get_List_Of_Orders()
    {
        //Act
        var result = await OrderAppService.ListAsync(new FilteredPagedAndSortedResultRequestDto());

        //Assert
        result.TotalCount.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task Should_Create_A_Valid_Order()
    {
        var result = await OrderAppService.CreateAsync(new OrderCreateDto()
        {
            Number = "Yeni sipariş",
            Statu = OrderStatu.Offer,
            Type = OrderType.Sales,
            Lines = new OrderLineCreateDto[]
            {
                new OrderLineCreateDto()
                {
                    Type = OrderLineType.Item,
                    ProductCode = "Malzeme-1",
                    Quantity = 10,
                    UnitCode = "Alt birim-1"
                },
                new OrderLineCreateDto()
                {
                    Type = OrderLineType.Service,
                    ProductCode = "Hizmet-1",
                    Quantity = 10,
                    UnitCode = "Alt birim-3"
                }
            }
        });

        result.Id.ShouldNotBe(0);
        result.Number.ShouldBe("Yeni sipariş");
        result.Lines.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task Should_Reserve_Items()
    {
        var result = await OrderAppService.CreateAsync(new OrderCreateDto()
        {
            Number = "Yeni sipariş",
            Statu = OrderStatu.Offer,
            Type = OrderType.Sales,
            Lines = new OrderLineCreateDto[]
            {
                new OrderLineCreateDto()
                {
                    Type = OrderLineType.Item,
                    ProductCode = "Malzeme-1",
                    Quantity = 10,
                    UnitCode = "Alt birim-1",
                    ReserveDate = System.DateTime.Now,
                    ReserveQuantity = 10
                },
                new OrderLineCreateDto()
                {
                    Type = OrderLineType.Service,
                    ProductCode = "Hizmet-1",
                    Quantity = 10,
                    UnitCode = "Alt birim-3"
                }
            }
        });

        result.Id.ShouldNotBe(0);
        result.Number.ShouldBe("Yeni sipariş");
        result.Lines.Count.ShouldBeGreaterThan(0);

        var itemAppService = GetRequiredService<IItemAppService>();
        var itemResult = await itemAppService.ListAsync(
            new FilteredPagedAndSortedResultRequestDto
            {
                Conditions = new Condition("Code", Operator.Equals, "Malzeme-1")
            });
        itemResult.Items.FirstOrDefault().ReservedStock.ShouldBe(-10);
    }

    [Fact]
    public async Task Should_Not_Create_Existing_Order_Number()
    {
        var exception = await Assert.ThrowsAsync<NumberAlreadyExistsException>(async () =>
         {
             await OrderAppService.CreateAsync(new OrderCreateDto()
             {
                 Number = "Sipariş-1",
                 Statu = OrderStatu.Offer,
                 Type = OrderType.Sales,
                 Lines = new OrderLineCreateDto[]
                 {
                    new OrderLineCreateDto()
                    {
                        Type = OrderLineType.Item,
                        ProductCode = "Malzeme-1",
                        Quantity = 10,
                        UnitCode = "Alt birim-1"
                    }
                 }
             });
         });

        exception.EntityNumber.ShouldBe("Sipariş-1");
    }

    [Fact]
    public async Task Should_Not_Create_When_Item_Not_Found()
    {
        var exception = await Assert.ThrowsAsync<CodeNotFoundException>(async () =>
         {
             await OrderAppService.CreateAsync(new OrderCreateDto()
             {
                 Number = "Yeni-Sipariş",
                 Statu = OrderStatu.Offer,
                 Type = OrderType.Sales,
                 Lines = new OrderLineCreateDto[]
                 {
                    new OrderLineCreateDto()
                    {
                        Type = OrderLineType.Item,
                        ProductCode = "Malzeme-None",
                        Quantity = 10,
                        UnitCode = "Alt birim-1"
                    }
                 }
             });
         });

        exception.EntityCode.ShouldBe("Malzeme-None");
    }

    [Fact]
    public async Task Should_Not_Create_When_Unit_Code_Not_Found()
    {
        var exception = await Assert.ThrowsAsync<CodeNotFoundException>(async () =>
        {
            await OrderAppService.CreateAsync(new OrderCreateDto()
            {
                Number = "Yeni-Sipariş",
                Statu = OrderStatu.Offer,
                Type = OrderType.Sales,
                Lines = new OrderLineCreateDto[]
                {
                    new OrderLineCreateDto()
                    {
                        Type = OrderLineType.Item,
                        ProductCode = "Malzeme-1",
                        Quantity = 10,
                        UnitCode = "Alt birim-yok"
                    }
                }
            });
        });

        exception.EntityCode.ShouldBe("Alt birim-yok");
    }

    [Fact]
    public async Task Should_Not_Create_When_Order_Line_Quantity_Zero_Or_Less()
    {
        var exception = await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await OrderAppService.CreateAsync(new OrderCreateDto()
            {
                Number = "Yeni-Sipariş",
                Statu = OrderStatu.Offer,
                Type = OrderType.Sales,
                Lines = new OrderLineCreateDto[]
                {
                    new OrderLineCreateDto()
                    {
                        Type = OrderLineType.Item,
                        ProductCode = "Malzeme-1",
                        Quantity = -1,
                        UnitCode = "Alt birim-2"
                    }
                }
            });
        });

        exception.Code.ShouldBe(SalerDomainErrorCodes.QuantityCannotZeroOrLess);
    }

    [Fact]
    public async Task Should_Not_Create_When_Client_Code_Not_Found()
    {
        var exception = await Assert.ThrowsAsync<CodeNotFoundException>(async () =>
        {
            await OrderAppService.CreateAsync(new OrderCreateDto()
            {
                Number = "Yeni-Sipariş",
                Statu = OrderStatu.Offer,
                Type = OrderType.Sales,
                ClientCode = "Cari yok",
                Lines = new OrderLineCreateDto[]
                {
                    new OrderLineCreateDto()
                    {
                        Type = OrderLineType.Item,
                        ProductCode = "Malzeme-1",
                        Quantity = 1,
                        UnitCode = "Alt birim-yok"
                    }
                }
            });
        });

        exception.EntityCode.ShouldBe("Cari yok");
    }

    [Fact]
    public async Task Should_Update_A_Valid_Order()
    {
        var result = await OrderAppService.UpdateAsync(
            1,
            new OrderUpdateDto()
            {
                Number = "Yeni sipariş",
                Statu = OrderStatu.Offer,
                Lines = new OrderLineUpdateDto[]
                {
                    new OrderLineUpdateDto()
                    {
                        Id = 1,
                        Type = OrderLineType.Item,
                        ProductCode = "Malzeme-1",
                        Quantity = 10,
                        UnitCode = "Alt birim-1"
                    },
                    new OrderLineUpdateDto()
                    {
                        Type = OrderLineType.Service,
                        ProductCode = "Hizmet-1",
                        Quantity = 10,
                        UnitCode = "Alt birim-3"
                    },
                    new OrderLineUpdateDto()
                    {
                        Type = OrderLineType.Item,
                        ProductCode = "Malzeme-3",
                        Quantity = 15,
                        UnitCode = "Alt birim-2"
                    }
                }
            });

        result.Lines.Count.Equals(3);
        Assert.DoesNotContain(result.Lines, line => line.Id == default);
    }

    [Fact]
    public async Task Should_Update_With_Correct_Line_Index()
    {
        //https://github.com/allegorysoft/saler/issues/106
        var result = await OrderAppService.UpdateAsync(
            20,
            new OrderUpdateDto()
            {
                Number = "Yeni sipariş",
                Statu = OrderStatu.Offer,
                Lines = new OrderLineUpdateDto[]
                {
                    new OrderLineUpdateDto()
                    {
                        Type = OrderLineType.Item,
                        ProductCode = "Malzeme-10",
                        Quantity = 10,
                        UnitCode = "Alt birim-1"
                    },
                    new OrderLineUpdateDto()
                    {
                        Type = OrderLineType.Service,
                        ProductCode = "Hizmet-1",
                        Quantity = 20,
                        UnitCode = "Alt birim-3"
                    },
                    new OrderLineUpdateDto()
                    {
                        Type = OrderLineType.Item,
                        ProductCode = "Malzeme-23",
                        Quantity = 30,
                        UnitCode = "Alt birim-2"
                    }
                }
            });

        result.Lines[0].Quantity.ShouldBe(10);
        result.Lines[1].Quantity.ShouldBe(20);
        result.Lines[2].Quantity.ShouldBe(30);
    }

    [Fact]
    public async Task Should_Not_Update_Existing_Order_Number()
    {
        var exception = await Assert.ThrowsAsync<NumberAlreadyExistsException>(async () =>
        {
            await OrderAppService.UpdateAsync(
                1,
                new OrderUpdateDto()
                {
                    Number = "Sipariş-3",
                    Statu = OrderStatu.Offer,
                    Lines = new OrderLineUpdateDto[]
                    {
                        new OrderLineUpdateDto()
                        {
                            Type = OrderLineType.Item,
                            ProductCode = "Malzeme-1",
                            Quantity = 10,
                            UnitCode = "Alt birim-1"
                        }
                    }
                });
        });

        exception.EntityNumber.ShouldBe("Sipariş-3");
    }

    [Fact]
    public async Task Should_Not_Update_When_Item_Not_Found()
    {
        var exception = await Assert.ThrowsAsync<CodeNotFoundException>(async () =>
        {
            await OrderAppService.UpdateAsync(
                1,
                new OrderUpdateDto()
                {
                    Number = "Yeni-Sipariş",
                    Statu = OrderStatu.Offer,
                    Lines = new OrderLineUpdateDto[]
                    {
                        new OrderLineUpdateDto()
                        {
                            Type = OrderLineType.Item,
                            ProductCode = "Malzeme-None",
                            Quantity = 10,
                            UnitCode = "Alt birim-1"
                        }
                    }
                });
        });

        exception.EntityCode.ShouldBe("Malzeme-None");
    }

    [Fact]
    public async Task Should_Not_Update_When_Unit_Code_Not_Found()
    {
        var exception = await Assert.ThrowsAsync<CodeNotFoundException>(async () =>
        {
            await OrderAppService.UpdateAsync(
                1,
                new OrderUpdateDto()
                {
                    Number = "Yeni-Sipariş",
                    Statu = OrderStatu.Offer,
                    Lines = new OrderLineUpdateDto[]
                    {
                        new OrderLineUpdateDto()
                        {
                            Type = OrderLineType.Item,
                            ProductCode = "Malzeme-1",
                            Quantity = 10,
                            UnitCode = "Alt birim-yok"
                        }
                    }
                });
        });

        exception.EntityCode.ShouldBe("Alt birim-yok");
    }

    [Fact]
    public async Task Should_Not_Update_When_Order_Line_Quantity_Zero_Or_Less()
    {
        var exception = await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await OrderAppService.UpdateAsync(
                1,
                new OrderUpdateDto()
                {
                    Number = "Yeni-Sipariş",
                    Statu = OrderStatu.Offer,
                    Lines = new OrderLineUpdateDto[]
                    {
                        new OrderLineUpdateDto()
                        {
                            Type = OrderLineType.Item,
                            ProductCode = "Malzeme-1",
                            Quantity = -1,
                            UnitCode = "Alt birim-1"
                        }
                    }
                });
        });

        exception.Code.ShouldBe(SalerDomainErrorCodes.QuantityCannotZeroOrLess);
    }

    [Fact]
    public async Task Should_Not_Update_When_Client_Code_Not_Found()
    {
        var exception = await Assert.ThrowsAsync<CodeNotFoundException>(async () =>
        {
            await OrderAppService.UpdateAsync(
                1,
                new OrderUpdateDto()
                {
                    Number = "Yeni-Sipariş",
                    Statu = OrderStatu.Offer,
                    ClientCode = "Cari yok",
                    Lines = new OrderLineUpdateDto[]
                    {
                        new OrderLineUpdateDto()
                        {
                            Type = OrderLineType.Item,
                            ProductCode = "Malzeme-1",
                            Quantity = 1,
                            UnitCode = "Alt birim-yok"
                        }
                    }
                });
        });

        exception.EntityCode.ShouldBe("Cari yok");
    }
}
