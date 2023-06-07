//#define seed_big
using Allegory.Saler.Clients;
using Allegory.Saler.Currencies;
using Allegory.Saler.Items;
using Allegory.Saler.Orders;
using Allegory.Saler.Services;
using Allegory.Saler.UnitPrices;
using Allegory.Saler.Units;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace Allegory.Saler;

#if seed_big
public class SalerBigDataSeederContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IUnitGroupRepository _unitGroupRepository;
    private readonly UnitGroupManager _unitGroupManager;
    private readonly IClientRepository _clientRepository;
    private readonly ClientManager _clientManager;
    private readonly IItemRepository _itemRepository;
    private readonly ItemManager _itemManager;
    private readonly IServiceRepository _serviceRepository;
    private readonly ServiceManager _serviceManager;
    private readonly IOrderRepository _orderRepository;
    private readonly OrderManager _orderManager;
    private readonly IServiceProvider _serviceProvider;

    public SalerBigDataSeederContributor(
        IUnitGroupRepository unitGroupRepository,
        UnitGroupManager unitGroupManager,
        IClientRepository clientRepository,
        ClientManager clientManager,
        IItemRepository itemRepository,
        ItemManager itemManager,
        IServiceRepository serviceRepository,
        ServiceManager serviceManager,
        IOrderRepository orderRepository,
        OrderManager orderManager,
        IServiceProvider serviceProvider)
    {
        _unitGroupRepository = unitGroupRepository;
        _unitGroupManager = unitGroupManager;
        _clientRepository = clientRepository;
        _clientManager = clientManager;
        _itemRepository = itemRepository;
        _itemManager = itemManager;
        _serviceRepository = serviceRepository;
        _serviceManager = serviceManager;
        _orderRepository = orderRepository;
        _orderManager = orderManager;
        _serviceProvider = serviceProvider;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        if (await _unitGroupRepository.GetCountAsync() == 0)
        {
            var entities = await CreateUnitGroups(30);
            await InsertPieceByPiece<IUnitGroupRepository, UnitGroup>(entities, 5);
        }

        if (await _clientRepository.GetCountAsync() == 0)
        {
            var entities = await CreateClients(200000);
            await InsertPieceByPiece<IClientRepository, Client>(entities, 100);
        }

        if (await _itemRepository.GetCountAsync() == 0)
        {
            var entities = await CreateItems(300000);
            await InsertPieceByPiece<IItemRepository, Item>(entities, 100);
        }

        if (await _serviceRepository.GetCountAsync() == 0)
        {
            var entities = await CreateServices(300000);
            await InsertPieceByPiece<IServiceRepository, Service>(entities, 100);
        }

        if (await _orderRepository.GetCountAsync() == 0)
        {
            var entities = await CreateOrders(1000);
            await InsertPieceByPiece<IOrderRepository, Order>(entities, 20);
        }
    }
    async Task InsertPieceByPiece<Repository, TEntity>(
        List<TEntity> entityList,
        int pieceRange)
        where Repository : class, IRepository<TEntity>
        where TEntity : class, IEntity
    {
        int loopCount = entityList.Count / pieceRange;
        for (int i = 0; i < loopCount; i++)
        {
            using (var service = _serviceProvider.CreateScope())
            {
                var repository = service.ServiceProvider.GetService<Repository>();
                int startRange = pieceRange * i;
                var insertedList = entityList.Skip(startRange).Take(pieceRange);
                await repository.InsertManyAsync(insertedList, true);
            }
        }
    }

    async Task<List<UnitGroup>> CreateUnitGroups(int range)
    {
        List<UnitGroup> unitGroups = new List<UnitGroup>();

        for (int i = 1; i <= range; i++)
        {
            List<Unit> units = new List<Unit>();
            for (int y = 1; y <= i + 2; y++)
            {
                var unit = new Unit(
                    "Alt birim-" + y.ToString(),
                    1,
                    y,
                    name: "alt birim-" + y.ToString(),
                    mainUnit: y == 1);

                units.Add(unit);
            }

            var unitGroup = new UnitGroup(
                "Ana birim-" + i.ToString(),
                name: "alt birim-" + i.ToString());
            unitGroup.UnitLines = units;

            unitGroups.Add(unitGroup);
        }

        return unitGroups;
    }

    async Task<List<Client>> CreateClients(int range)
    {
        List<Client> clients = new List<Client>();

        for (int i = 1; i <= range; i++)
        {
            var client = new Client(
                "Müşteri-" + i.ToString(),
                title: "unvan kod-" + i.ToString(),
                i.ToString().PadLeft(10, '0'));

            clients.Add(client);
        }

        return clients;
    }

    async Task<List<Item>> CreateItems(int range)
    {
        List<Item> items = new List<Item>();

        for (int i = 1; i <= range; i++)
        {
            var item = new Item(
                "Malzeme-" + i.ToString(),
                ItemType.Item,
                1,
                name: "açıklama " + i.ToString().PadLeft(10, '0'));

            items.Add(item);
        }

        return items;
    }

    async Task<List<Service>> CreateServices(int range)
    {
        List<Service> services = new List<Service>();

        for (int i = 1; i <= range; i++)
        {
            var service = new Service(
                "Hizmet-" + i.ToString(),
                1,
                name: "açıklama " + i.ToString().PadLeft(10, '0'));

            services.Add(service);
        }

        return services;
    }

    async Task<List<Order>> CreateOrders(int range)
    {
        List<Order> orders = new List<Order>();
        for (int i = 1; i <= range; i++)
        {
            var order = new Order(
                "Sipariş-" + i.ToString(),
                i % 2 == 0 ? OrderType.Purchase : OrderType.Sales,
                i % 2 == 0 ? OrderStatu.Offer : OrderStatu.Approved);

            for (int y = 1; y <= (i > 20 ? 20 : i); y++)
            {
                var orderLine = new OrderLine();
                orderLine.Type = y % 2 == 0 ? OrderLineType.Item : OrderLineType.Service;
                orderLine.productId = y % 2 == 0 ? 10 : 20;
                orderLine.SetQuantity(y);
                orderLine.UnitId = y % 2 == 0 ? 1 : 2;



                order.Lines.Add(orderLine);
            }

            orders.Add(order);
        }

        return orders;
    }
}
#else
public class SalerDataSeederContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IUnitGroupRepository _unitGroupRepository;
    private readonly UnitGroupManager _unitGroupManager;
    private readonly IClientRepository _clientRepository;
    private readonly ClientManager _clientManager;
    private readonly IItemRepository _itemRepository;
    private readonly ItemManager _itemManager;
    private readonly IServiceRepository _serviceRepository;
    private readonly ServiceManager _serviceManager;
    private readonly IOrderRepository _orderRepository;
    private readonly OrderManager _orderManager;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly CurrencyManager _currencyManager;
    private readonly IUnitPriceRepository _unitPriceRepository;
    private readonly UnitPriceManager _unitPriceManager;

    public SalerDataSeederContributor(
        IUnitGroupRepository unitGroupRepository,
        UnitGroupManager unitGroupManager,
        IClientRepository clientRepository,
        ClientManager clientManager,
        IItemRepository itemRepository,
        ItemManager itemManager,
        IServiceRepository serviceRepository,
        ServiceManager serviceManager,
        IOrderRepository orderRepository,
        OrderManager orderManager,
        ICurrencyRepository currencyRepository,
        CurrencyManager currencyManager,
        IUnitPriceRepository unitPriceRepository,
        UnitPriceManager unitPriceManager)
    {
        _unitGroupRepository = unitGroupRepository;
        _unitGroupManager = unitGroupManager;
        _clientRepository = clientRepository;
        _clientManager = clientManager;
        _itemRepository = itemRepository;
        _itemManager = itemManager;
        _serviceRepository = serviceRepository;
        _serviceManager = serviceManager;
        _orderRepository = orderRepository;
        _orderManager = orderManager;
        _currencyRepository = currencyRepository;
        _currencyManager = currencyManager;
        _unitPriceRepository = unitPriceRepository;
        _unitPriceManager = unitPriceManager;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        if (await _unitGroupRepository.GetCountAsync() == 0)
            await _unitGroupRepository.InsertManyAsync(await CreateUnitGroups(3), true);

        if (await _clientRepository.GetCountAsync() == 0)
            await _clientRepository.InsertManyAsync(await CreateClients(50), true);

        if (await _itemRepository.GetCountAsync() == 0)
            await _itemRepository.InsertManyAsync(await CreateItems(50), true);

        if (await _serviceRepository.GetCountAsync() == 0)
            await _serviceRepository.InsertManyAsync(await CreateServices(50), true);

        if (await _orderRepository.GetCountAsync() == 0)
            await _orderRepository.InsertManyAsync(await CreateOrders(50));

        if (await _currencyRepository.GetCountAsync() == 0)
            await _currencyRepository.InsertManyAsync(await CreateCurrencies());

        if (await _unitPriceRepository.GetCountAsync() == 0)
            await _unitPriceRepository.InsertManyAsync(await CreateUnitPrices(50));
    }

    async Task<List<UnitGroup>> CreateUnitGroups(int range)
    {
        List<UnitGroup> unitGroups = new List<UnitGroup>();

        for (int i = 1; i <= range; i++)
        {
            List<Unit> units = new List<Unit>();
            for (int y = 1; y <= i + 2; y++)
            {
                var unit = new Unit(
                    "Alt birim-" + y.ToString(),
                    1,
                    y,
                    true,
                    name: "alt birim-" + y.ToString(),
                    mainUnit: y == 1);

                units.Add(unit);
            }
            unitGroups.Add(await _unitGroupManager.CreateAsync(
                "Ana birim-" + i.ToString(),
                units,
                name: "ana birim-" + i.ToString()));
        }

        return unitGroups;
    }

    async Task<List<Client>> CreateClients(int range)
    {
        List<Client> clients = new List<Client>();

        for (int i = 1; i <= range; i++)
        {
            clients.Add(await _clientManager.CreateAsync(
                "Müşteri-" + i.ToString(),
                title: "unvan kod-" + i.ToString(),
                i.ToString().PadLeft(10, '0')));
        }

        return clients;
    }

    async Task<List<Item>> CreateItems(int range)
    {
        List<Item> items = new List<Item>();

        for (int i = 1; i <= range; i++)
        {
            var item = await _itemManager.CreateAsync(
                "Malzeme-" + i.ToString(),
                ItemType.Item,
                "Ana birim-1",
                name: "açıklama " + i.ToString().PadLeft(10, '0'));
            item.SetSalesVatRate(18);
            item.SetPurchaseVatRate(18);

            items.Add(item);
        }

        return items;
    }

    async Task<List<Service>> CreateServices(int range)
    {
        List<Service> services = new List<Service>();

        for (int i = 1; i <= range; i++)
        {
            var service = await _serviceManager.CreateAsync(
                "Hizmet-" + i.ToString(),
                "Ana birim-1",
                name: "açıklama " + i.ToString().PadLeft(10, '0'));
            service.SetSalesVatRate(18);
            service.SetPurchaseVatRate(18);

            services.Add(service);
        }

        return services;
    }

    async Task<List<Order>> CreateOrders(int range)
    {
        List<Order> orders = new List<Order>();

        for (int i = 1; i <= range; i++)
        {
            var order = await _orderManager.CreateAsync(
                "Sipariş-" + i.ToString(),
                i % 2 == 0 ? OrderType.Purchase : OrderType.Sales,
                DateTime.Now,
                i % 2 == 0 ? OrderStatu.Offer : OrderStatu.Approved);

            for (int y = 1; y <= i; y++)
            {
                var orderLine = await _orderManager.CreateOrderLineAsync(
                    y % 2 == 0 ? OrderLineType.Item : OrderLineType.Service,
                    (y % 2 == 0 ? "Malzeme-" : "Hizmet-") + y.ToString(),
                    y,
                    y % 2 == 0 ? "Alt birim-1" : "Alt birim-2");

                order.Lines.Add(orderLine);
            }

            orders.Add(order);
        }

        return orders;
    }

    async Task<List<Currency>> CreateCurrencies()
    {
        List<Currency> currencies = new List<Currency>();
        currencies.Add(await _currencyManager.CreateAsync("USD", "ABD Doları", "$"));
        currencies.Add(await _currencyManager.CreateAsync("EUR", "Euro", "€"));
        currencies.Add(await _currencyManager.CreateAsync("GBP", "İngiliz Sterlini", "£"));
        currencies.Add(await _currencyManager.CreateAsync("ADP", "Angola Kwanzası"));
        currencies.Add(await _currencyManager.CreateAsync("AED", "BAE Dirhemi"));
        currencies.Add(await _currencyManager.CreateAsync("AFN", "Afganistan Afganisi"));
        currencies.Add(await _currencyManager.CreateAsync("ALL", "Arnavutluk Leki"));
        currencies.Add(await _currencyManager.CreateAsync("AMD", "Ermeni Dramı"));
        currencies.Add(await _currencyManager.CreateAsync("ANG", "Hollanda Antilleri Florini"));
        currencies.Add(await _currencyManager.CreateAsync("AOA", "Andorra Pesetası"));
        currencies.Add(await _currencyManager.CreateAsync("ARS", "Arjantin Pesosu"));
        currencies.Add(await _currencyManager.CreateAsync("ATS", "Avusturya Şilini"));
        currencies.Add(await _currencyManager.CreateAsync("AUD", "Avustralya Doları"));
        currencies.Add(await _currencyManager.CreateAsync("AWG", "Aruba Florini"));
        currencies.Add(await _currencyManager.CreateAsync("AZM", "Azerbaycan Manatı"));
        currencies.Add(await _currencyManager.CreateAsync("AZN", "Azerbaycan Yeni Manatı", "Manat"));
        currencies.Add(await _currencyManager.CreateAsync("BBD", "Barbados Doları"));
        currencies.Add(await _currencyManager.CreateAsync("BDT", "Bengaldeş Takası"));
        currencies.Add(await _currencyManager.CreateAsync("BEF", "Belçika Frangı", "BF"));
        currencies.Add(await _currencyManager.CreateAsync("BGN", "Bulgar Levası"));
        currencies.Add(await _currencyManager.CreateAsync("BHD", "Bahreyn Dinarı"));
        currencies.Add(await _currencyManager.CreateAsync("BIF", "Burundi Frangı"));
        currencies.Add(await _currencyManager.CreateAsync("BMD", "Bermuda Doları"));
        currencies.Add(await _currencyManager.CreateAsync("BND", "Brunei Doları"));
        currencies.Add(await _currencyManager.CreateAsync("BOB", "Bolivya Bolivianosu"));
        currencies.Add(await _currencyManager.CreateAsync("BRL", "Brezilya Cruzeirosu"));
        currencies.Add(await _currencyManager.CreateAsync("BSD", "Bahama Doları"));
        currencies.Add(await _currencyManager.CreateAsync("BTN", "Butan Lirası"));
        currencies.Add(await _currencyManager.CreateAsync("BWP", "Botswana Pulası"));
        currencies.Add(await _currencyManager.CreateAsync("BYR", "Beyaz Rusya Rublesi"));
        currencies.Add(await _currencyManager.CreateAsync("BZD", "Belize Doları"));
        currencies.Add(await _currencyManager.CreateAsync("CAD", "Kanada Doları"));
        currencies.Add(await _currencyManager.CreateAsync("CDF", "Kongo Frangı"));
        currencies.Add(await _currencyManager.CreateAsync("CHF", "İsviçre Frangı", "SF"));
        currencies.Add(await _currencyManager.CreateAsync("CLP", "Şili Pesosu"));
        currencies.Add(await _currencyManager.CreateAsync("CNY", "Çin Yüeni"));
        currencies.Add(await _currencyManager.CreateAsync("COP", "Kolombiya Pesosu"));
        currencies.Add(await _currencyManager.CreateAsync("CRC", "Kosta Rika Kolonu"));
        currencies.Add(await _currencyManager.CreateAsync("CUP", "Küba Pesosu"));
        currencies.Add(await _currencyManager.CreateAsync("CVE", "Cape Verde Esküdosu"));
        currencies.Add(await _currencyManager.CreateAsync("CYP", "Kıbrıs Lirası"));
        currencies.Add(await _currencyManager.CreateAsync("CZK", "Çek Kuronu"));
        currencies.Add(await _currencyManager.CreateAsync("DEM", "Alman Markı", "DM"));
        currencies.Add(await _currencyManager.CreateAsync("DJF", "Cibuti Frangı"));
        currencies.Add(await _currencyManager.CreateAsync("DKK", "Danimarka Kronu", "DK"));
        currencies.Add(await _currencyManager.CreateAsync("DOP", "Dominik Pesosu"));
        currencies.Add(await _currencyManager.CreateAsync("DZD", "Cezayir Dinarı"));
        currencies.Add(await _currencyManager.CreateAsync("ECS", "Ekvator Sucresi"));
        currencies.Add(await _currencyManager.CreateAsync("EEK", "Estonya Kuronu"));
        currencies.Add(await _currencyManager.CreateAsync("EGP", "Mısır Lirası"));
        currencies.Add(await _currencyManager.CreateAsync("ERN", "Eritre Nakfası"));
        currencies.Add(await _currencyManager.CreateAsync("ESP", "İspanyol Pesetası"));
        currencies.Add(await _currencyManager.CreateAsync("ETB", "Etyopya Birri"));
        currencies.Add(await _currencyManager.CreateAsync("FIM", "Fin Markkası"));
        currencies.Add(await _currencyManager.CreateAsync("FJD", "Fiji Adaları Doları"));
        currencies.Add(await _currencyManager.CreateAsync("FKP", "Falkland Adaları Sterlini"));
        currencies.Add(await _currencyManager.CreateAsync("FRF", "Fransız Frangı", "FF"));
        currencies.Add(await _currencyManager.CreateAsync("GEL", "Gürcistan Larisi"));
        currencies.Add(await _currencyManager.CreateAsync("GHS", "Gana Cedisi"));
        currencies.Add(await _currencyManager.CreateAsync("GIP", "Cebelitarık Sterlini"));
        currencies.Add(await _currencyManager.CreateAsync("GMD", "Gambia Dalasisi"));
        currencies.Add(await _currencyManager.CreateAsync("GNF", "Gine Frangı"));
        currencies.Add(await _currencyManager.CreateAsync("GRD", "Yunan Drahmisi"));
        currencies.Add(await _currencyManager.CreateAsync("GTQ", "Guatemala Quetzali"));
        currencies.Add(await _currencyManager.CreateAsync("GWP", "Gine-Bisse Pesosu"));
        currencies.Add(await _currencyManager.CreateAsync("GYD", "Guyana Doları"));
        currencies.Add(await _currencyManager.CreateAsync("HKD", "Hongkong Doları"));
        currencies.Add(await _currencyManager.CreateAsync("HNL", "Honduras Lempirası"));
        currencies.Add(await _currencyManager.CreateAsync("HRK", "Hırvatistsan Kunası"));
        currencies.Add(await _currencyManager.CreateAsync("HTG", "Haiti Gourdesi"));
        currencies.Add(await _currencyManager.CreateAsync("HUF", "Macaristan Forinti"));
        currencies.Add(await _currencyManager.CreateAsync("IDR", "Endonezya Rupisi"));
        currencies.Add(await _currencyManager.CreateAsync("IEP", "İrlanda Lirası"));
        currencies.Add(await _currencyManager.CreateAsync("ILS", "İsrail Şekeli"));
        currencies.Add(await _currencyManager.CreateAsync("INR", "Hindistan Rupisi"));
        currencies.Add(await _currencyManager.CreateAsync("IQD", "Irak Dinarı"));
        currencies.Add(await _currencyManager.CreateAsync("IRR", "İran Riyali"));
        currencies.Add(await _currencyManager.CreateAsync("ISK", "İzlanda Kuronu"));
        currencies.Add(await _currencyManager.CreateAsync("ITL", "İtalyan Lireti", "IL"));
        currencies.Add(await _currencyManager.CreateAsync("JMD", "Jamaika Doları"));
        currencies.Add(await _currencyManager.CreateAsync("JOD", "Ürdün Dinarı"));
        currencies.Add(await _currencyManager.CreateAsync("JPY", "Japon Yeni"));
        currencies.Add(await _currencyManager.CreateAsync("KES", "Kenya Şilingi"));
        currencies.Add(await _currencyManager.CreateAsync("KGS", "Kırgızistan Somu"));
        currencies.Add(await _currencyManager.CreateAsync("KHR", "Kamboçya Rieli"));
        currencies.Add(await _currencyManager.CreateAsync("KM", "Konvertibıl Mark"));
        currencies.Add(await _currencyManager.CreateAsync("KMF", "Komor Frangi"));
        currencies.Add(await _currencyManager.CreateAsync("KPW", "Kuzey Kore Wonu"));
        currencies.Add(await _currencyManager.CreateAsync("KRW", "Güney Kore Wonu"));
        currencies.Add(await _currencyManager.CreateAsync("KWD", "Kuveyt Dinarı"));
        currencies.Add(await _currencyManager.CreateAsync("KYD", "Cayman Adaları Doları"));
        currencies.Add(await _currencyManager.CreateAsync("KZT", "Kazak Tengesi"));
        currencies.Add(await _currencyManager.CreateAsync("LAK", "Laos Kipi"));
        currencies.Add(await _currencyManager.CreateAsync("LBP", "Lübnan Lirası"));
        currencies.Add(await _currencyManager.CreateAsync("LKR", "Sri Lanka Rupisi"));
        currencies.Add(await _currencyManager.CreateAsync("LRD", "Liberya Doları"));
        currencies.Add(await _currencyManager.CreateAsync("LSL", "Lesoto Lotisi"));
        currencies.Add(await _currencyManager.CreateAsync("LTL", "Litvanya Litası"));
        currencies.Add(await _currencyManager.CreateAsync("LUF", "Lüksemburg Frangı"));
        currencies.Add(await _currencyManager.CreateAsync("LVL", "Letonya Latsı"));
        currencies.Add(await _currencyManager.CreateAsync("LYD", "Libya Dinarı"));
        currencies.Add(await _currencyManager.CreateAsync("MAD", "Fas Dirhemi"));
        currencies.Add(await _currencyManager.CreateAsync("MDL", "Moldova Leyi"));
        currencies.Add(await _currencyManager.CreateAsync("MGA", "Malgaş ariarysi"));
        currencies.Add(await _currencyManager.CreateAsync("MKD", "Makedonya Dinarı"));
        currencies.Add(await _currencyManager.CreateAsync("MMK", "Kyat"));
        currencies.Add(await _currencyManager.CreateAsync("MNT", "Moğol Tugriki"));
        currencies.Add(await _currencyManager.CreateAsync("MOP", "Macau Patacası"));
        currencies.Add(await _currencyManager.CreateAsync("MRO", "Moritanya Ogiyası"));
        currencies.Add(await _currencyManager.CreateAsync("MTL", "Malta Lirası"));
        currencies.Add(await _currencyManager.CreateAsync("MUR", "Mauritius Rupisi"));
        currencies.Add(await _currencyManager.CreateAsync("MVR", "Maldiv Rufiyası"));
        currencies.Add(await _currencyManager.CreateAsync("MWK", "Malavi Kwachası"));
        currencies.Add(await _currencyManager.CreateAsync("MXN", "Meksika Pesosu"));
        currencies.Add(await _currencyManager.CreateAsync("MYR", "Malezya Ringgiti"));
        currencies.Add(await _currencyManager.CreateAsync("MZN", "Mozambik Meticali"));
        currencies.Add(await _currencyManager.CreateAsync("NAD", "Namibya Doları"));
        currencies.Add(await _currencyManager.CreateAsync("NGN", "Nijerya Nairası"));
        currencies.Add(await _currencyManager.CreateAsync("NIO", "Nikaragua Cordoba Orosu"));
        currencies.Add(await _currencyManager.CreateAsync("NLG", "Hollanda Florini", "NG"));
        currencies.Add(await _currencyManager.CreateAsync("NOK", "Norveç Kronu"));
        currencies.Add(await _currencyManager.CreateAsync("NPR", "Nepal Rupisi"));
        currencies.Add(await _currencyManager.CreateAsync("NZD", "Yeni Zelanda Doları"));
        currencies.Add(await _currencyManager.CreateAsync("OMR", "Umman Riyali"));
        currencies.Add(await _currencyManager.CreateAsync("PAB", "Panama Balboası"));
        currencies.Add(await _currencyManager.CreateAsync("PEN", "Peru Solu"));
        currencies.Add(await _currencyManager.CreateAsync("PGK", "Papua Yeni Gine Kinası"));
        currencies.Add(await _currencyManager.CreateAsync("PHP", "Filipin Pesosu"));
        currencies.Add(await _currencyManager.CreateAsync("PKR", "Pakistan Rupisi"));
        currencies.Add(await _currencyManager.CreateAsync("PLN", "Polonya Zlotisi"));
        currencies.Add(await _currencyManager.CreateAsync("PTE", "Portekiz Escudosu"));
        currencies.Add(await _currencyManager.CreateAsync("PYG", "Paraguay Guaranisi"));
        currencies.Add(await _currencyManager.CreateAsync("QAR", "Katar Riyali"));
        currencies.Add(await _currencyManager.CreateAsync("ROL", "Romen Leyi"));
        currencies.Add(await _currencyManager.CreateAsync("RON", "Romen Yeni Leyi"));
        currencies.Add(await _currencyManager.CreateAsync("RSD", "Sırp Dinarı"));
        currencies.Add(await _currencyManager.CreateAsync("RUB", "Rus Rublesi"));
        currencies.Add(await _currencyManager.CreateAsync("RWF", "Ruanda Frangı"));
        currencies.Add(await _currencyManager.CreateAsync("SAR", "S. Arabistan Riyali"));
        currencies.Add(await _currencyManager.CreateAsync("SBD", "Solomon Adaları Doları"));
        currencies.Add(await _currencyManager.CreateAsync("SCR", "Seyşel Adaları Rupisi"));
        currencies.Add(await _currencyManager.CreateAsync("SDG", "Sudan Dinarı"));
        currencies.Add(await _currencyManager.CreateAsync("SEK", "İsveç Kronu", "SK"));
        currencies.Add(await _currencyManager.CreateAsync("SGD", "Singapur Doları"));
        currencies.Add(await _currencyManager.CreateAsync("SHP", "St. Helen Lirası"));
        currencies.Add(await _currencyManager.CreateAsync("SLL", "Sierra Leone Leonesi"));
        currencies.Add(await _currencyManager.CreateAsync("SOS", "Somali Şilini"));
        currencies.Add(await _currencyManager.CreateAsync("SRD", "Surinam Florini"));
        currencies.Add(await _currencyManager.CreateAsync("STD", "Sao Tome Dobrası"));
        currencies.Add(await _currencyManager.CreateAsync("SVC", "El Salvador Colonu"));
        currencies.Add(await _currencyManager.CreateAsync("SYP", "Suriye Lirası"));
        currencies.Add(await _currencyManager.CreateAsync("SZL", "Swaziland Lilangenisi"));
        currencies.Add(await _currencyManager.CreateAsync("THB", "Tayland Bahtı"));
        currencies.Add(await _currencyManager.CreateAsync("TJS", "Somoni"));
        currencies.Add(await _currencyManager.CreateAsync("TL", "Türk Lirası"));
        currencies.Add(await _currencyManager.CreateAsync("TMT", "Türkmenistan Manatı"));
        currencies.Add(await _currencyManager.CreateAsync("TND", "Tunus Dinarı"));
        currencies.Add(await _currencyManager.CreateAsync("TOP", "Pa'anga"));
        currencies.Add(await _currencyManager.CreateAsync("TPE", "Doğu Timor Esküdosu"));
        currencies.Add(await _currencyManager.CreateAsync("TRY", "Türk Lirası"));
        currencies.Add(await _currencyManager.CreateAsync("TTD", "Trinidad ve Tobago Doları"));
        currencies.Add(await _currencyManager.CreateAsync("TWD", "Tayvan Doları"));
        currencies.Add(await _currencyManager.CreateAsync("TZS", "Tanzanya Şilini"));
        currencies.Add(await _currencyManager.CreateAsync("UAH", "Ukrayna Grevniyası"));
        currencies.Add(await _currencyManager.CreateAsync("UGX", "Uganda Şilini"));
        currencies.Add(await _currencyManager.CreateAsync("UYU", "Uruguay Pesosu"));
        currencies.Add(await _currencyManager.CreateAsync("UZS", "Özbekistan Somu"));
        currencies.Add(await _currencyManager.CreateAsync("VEB", "Venezuella Bolivarı"));
        currencies.Add(await _currencyManager.CreateAsync("VEF", "Venezuela Bolivarı"));
        currencies.Add(await _currencyManager.CreateAsync("VND", "Vietnam Dongu"));
        currencies.Add(await _currencyManager.CreateAsync("VUV", "Vanuatu Vatusu"));
        currencies.Add(await _currencyManager.CreateAsync("WST", "Samoa Talası"));
        currencies.Add(await _currencyManager.CreateAsync("XAF", "Central African CFA Franc"));
        currencies.Add(await _currencyManager.CreateAsync("XCD", "Doğu Karayip Doları"));
        currencies.Add(await _currencyManager.CreateAsync("XEU", "Avrupa Para Birimi"));
        currencies.Add(await _currencyManager.CreateAsync("XOF", "CFA Frangı"));
        currencies.Add(await _currencyManager.CreateAsync("XPF", "CFP Frangı"));
        currencies.Add(await _currencyManager.CreateAsync("YDD", "Yemen Dinarı"));
        currencies.Add(await _currencyManager.CreateAsync("YER", "Yemen Riyali"));
        currencies.Add(await _currencyManager.CreateAsync("YUD", "Yugoslav Dinarı"));
        currencies.Add(await _currencyManager.CreateAsync("ZAR", "Güney Afrika Randı"));
        currencies.Add(await _currencyManager.CreateAsync("ZMK", "Zambia Kwachası"));
        currencies.Add(await _currencyManager.CreateAsync("ZWL", "Zimbabwe Doları"));
        return currencies;
    }

    async Task<List<UnitPrice>> CreateUnitPrices(int range)
    {
        List<UnitPrice> unitPrices = new List<UnitPrice>();

        for (int i = 1; i <= range; i++)
        {
            var unitPrice = await _unitPriceManager.CreateAsync(
                "BF-" + i.ToString(),
                UnitPriceType.Item,
                "Malzeme-" + i,
                "Alt birim-1",
                i,
                i*2,
                DateTime.Now.Date.AddDays(-50),
                DateTime.Now.Date.AddDays(50));

            var unitPrice2 = await _unitPriceManager.CreateAsync(
                "BF-" + i.ToString(),
                UnitPriceType.Service,
                "Hizmet-" + i,
                "Alt birim-1",
                i,
                i * 2,
                DateTime.Now.Date.AddDays(-50),
                DateTime.Now.Date.AddDays(50));

            unitPrices.Add(unitPrice);
            unitPrices.Add(unitPrice2);
        }

        return unitPrices;
    }
}
#endif