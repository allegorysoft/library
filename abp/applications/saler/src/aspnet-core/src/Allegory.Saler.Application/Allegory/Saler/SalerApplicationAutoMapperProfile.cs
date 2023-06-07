using Allegory.Saler.Calculations.Product;
using Allegory.Saler.Clients;
using Allegory.Saler.Currencies;
using Allegory.Saler.Items;
using Allegory.Saler.Orders;
using Allegory.Saler.Services;
using Allegory.Saler.UnitPrices;
using Allegory.Saler.Units;
using AutoMapper;

namespace Allegory.Saler;

public class SalerApplicationAutoMapperProfile : Profile
{
    public SalerApplicationAutoMapperProfile()
    {
        CreateMap<UnitGroup, UnitGroupDto>();
        CreateMap<UnitGroup, UnitGroupWithDetailsDto>();
        CreateMap<Unit, UnitDto>();
        CreateMap<GlobalUnit, GlobalUnitDto>();

        CreateMap<Client, ClientDto>();
        CreateMap<Client, ClientWithDetailsDto>();

        CreateMap<Item, ItemDto>();
        CreateMap<Item, ItemWithDetailsDto>();

        CreateMap<Service, ServiceDto>();
        CreateMap<Service, ServiceWithDetailsDto>();

        CreateMap<Order, OrderDto>();
        CreateMap<Order, OrderWithDetailsDto>();
        CreateMap<OrderLine, OrderLineDto>();

        CreateMap<Discount, DiscountDto>()
            .ReverseMap()
            .ConstructUsing(dto => new Discount());
        CreateMap<Deduction, DeductionDto>();
        CreateMap<CalculableProduct<Discount>, CalculableProductDto>().ReverseMap();
        CreateMap<
            CalculableProductsAggregateRoot<
                Discount,
                CalculableProduct<Discount>,
                Discount>,
            CalculableProductAggregateRootDto>();

        CreateMap<Currency, CurrencyDto>();
        CreateMap<CurrencyDailyExchange, CurrencyDailyExchangeDto>();

        CreateMap<UnitPrice, UnitPriceDto>();
        CreateMap<UnitPrice, UnitPriceWithDetailsDto>();
    }
}
