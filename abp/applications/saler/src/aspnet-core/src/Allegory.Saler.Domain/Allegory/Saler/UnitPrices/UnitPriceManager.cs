using Allegory.Saler.Clients;
using Allegory.Saler.Currencies;
using Allegory.Saler.Items;
using Allegory.Saler.Services;
using Allegory.Saler.Units;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Allegory.Saler.UnitPrices;

public class UnitPriceManager : SalerDomainService
{
    protected IUnitPriceRepository UnitPriceRepository { get; }
    protected IItemRepository ItemRepository => LazyServiceProvider.LazyGetRequiredService<IItemRepository>();
    protected IServiceRepository ServiceRepository => LazyServiceProvider.LazyGetRequiredService<IServiceRepository>();
    protected ICurrencyRepository CurrencyRepository => LazyServiceProvider.LazyGetRequiredService<ICurrencyRepository>();
    protected IClientRepository ClientRepository => LazyServiceProvider.LazyGetRequiredService<IClientRepository>();
    protected IReadOnlyRepository<Unit, int> UnitRepository => LazyServiceProvider.LazyGetRequiredService<IReadOnlyRepository<Unit, int>>();

    public UnitPriceManager(IUnitPriceRepository unitPriceRepository)
    {
        UnitPriceRepository = unitPriceRepository;
    }

    public async Task<UnitPrice> CreateAsync(
        string code,
        UnitPriceType type,
        string productCode,
        string unitCode,
        decimal purchasePrice,
        decimal salesPrice,
        DateTime beginDate,
        DateTime endDate,
        string currencyCode = default,
        string clientCode = default)
    {
        await CheckUnitPriceExistsAsync(code, type);

        var unitPrice = new UnitPrice(
            code,
            type);

        unitPrice.SetPrice(purchasePrice, salesPrice);
        unitPrice.SetDates(beginDate, endDate);

        await SetProductAsync(unitPrice, productCode, unitCode);
        await SetCurrencyAsync(unitPrice, currencyCode);
        await SetClientAsync(unitPrice, clientCode);

        return unitPrice;
    }

    public async Task CheckUnitPriceExistsAsync(
        string code,
        UnitPriceType type,
        int? unitPriceId = default)
    {
        Expression<Func<UnitPrice, bool>> expression = unitPrice =>
        unitPrice.Code == code && unitPrice.Type == type;

        if (unitPriceId != default)
            expression = expression.And(unitPrice => unitPrice.Id != unitPriceId);

        var unitPriceExists = await UnitPriceRepository.AnyAsync(expression);

        if (unitPriceExists)
            throw new CodeAlreadyExistsException(typeof(UnitPrice), code);
    }

    public async Task ChangeCodeAsync(
        UnitPrice unitPrice,
        string code)
    {
        await CheckUnitPriceExistsAsync(code, unitPrice.Type, unitPrice.Id);
        unitPrice.ChangeCode(code);
    }

    public async Task SetProductAsync(
        UnitPrice unitPrice,
        string productCode,
        string unitCode)
    {
        IEntity<int> product = null;
        Unit unit = null;

        switch (unitPrice.Type)
        {
            case UnitPriceType.Item:
                var item = await ItemRepository.GetByCodeAsync(productCode, false);
                product = item;
                unit = await SetUnitAsync(unitPrice, item.UnitGroupId, unitCode);
                break;

            case UnitPriceType.Service:
                var service = await ServiceRepository.GetByCodeAsync(productCode, false);
                product = service;
                unit = await SetUnitAsync(unitPrice, service.UnitGroupId, unitCode);
                break;
        }

        unitPrice.ProductId = product.Id;
    }

    protected async Task<Unit> SetUnitAsync(
        UnitPrice unitPrice,
        int unitGroupId,
        string unitCode)
    {
        var unit = await UnitRepository.FirstOrDefaultAsync(unit => unit.UnitGroupId == unitGroupId && unit.Code == unitCode);

        if (unit == null)
            throw new CodeNotFoundException(typeof(Unit), unitCode);

        unitPrice.UnitId = unit.Id;

        return unit;
    }

    public async Task SetCurrencyAsync(
        UnitPrice unitPrice,
        string currencyCode)
    {
        if (string.IsNullOrWhiteSpace(currencyCode))
            unitPrice.CurrencyId = null;
        else
        {
            var currency = await CurrencyRepository.GetByCodeAsync(currencyCode, false);
            unitPrice.CurrencyId = currency.Id;
        }
    }

    public async Task SetClientAsync(
        UnitPrice unitPrice,
        string clientCode)
    {
        if (string.IsNullOrWhiteSpace(clientCode))
            unitPrice.ClientId = null;
        else
        {
            var currency = await ClientRepository.GetByCodeAsync(clientCode, false);
            unitPrice.ClientId = currency.Id;
        }
    }

    #region Get price
    
    public async Task<decimal> GetPriceOldAsync(
        string productCode,
        UnitPriceType type,
        string unitCode,
        DateTime date,
        bool isSales,
        byte? vatRate = default,
        string currencyCode = default,
        string clientCode = default)
    {
        var query = from unitPrice in await UnitPriceRepository.GetQueryableAsync()

                    join unit in await UnitRepository.GetQueryableAsync()
                    on unitPrice.UnitId equals unit.Id

                    join currency in await CurrencyRepository.GetQueryableAsync()
                    on unitPrice.CurrencyId equals currency.Id into currencies
                    from currency in currencies.DefaultIfEmpty()

                    join client in await ClientRepository.GetQueryableAsync()
                    on unitPrice.ClientId equals client.Id into clients
                    from client in clients.DefaultIfEmpty()

                        #region Item/Service
                    join item in await ItemRepository.GetQueryableAsync()
                    on new
                    {
                        ProductId = unitPrice.ProductId,
                        Type = unitPrice.Type
                    }
                    equals new
                    {
                        ProductId = item.Id,
                        Type = UnitPriceType.Item
                    } into items
                    from item in items.DefaultIfEmpty()

                    join service in await ServiceRepository.GetQueryableAsync()
                    on new
                    {
                        ProductId = unitPrice.ProductId,
                        Type = unitPrice.Type
                    }
                    equals new
                    {
                        ProductId = service.Id,
                        Type = UnitPriceType.Service
                    } into services
                    from service in services.DefaultIfEmpty()
                        #endregion

                    where
                       unitPrice.Type == type
                    && unit.Code == unitCode
                    && date >= unitPrice.BeginDate
                    && date <= unitPrice.EndDate
                    && (productCode == (unitPrice.Type == UnitPriceType.Item
                                       ? item.Code : service.Code))
                    && (currencyCode.IsNullOrWhiteSpace()
                        ? unitPrice.CurrencyId == null : currency.Code == currencyCode)

                    && (clientCode.IsNullOrWhiteSpace()
                        ? unitPrice.ClientId == null
                        : (unitPrice.ClientId == null || client.Code == clientCode))

                    orderby client.Code descending

                    select new
                    {
                        Id = unitPrice.Id,
                        IsVatIncluded = unitPrice.IsVatIncluded,
                        SalesPrice = unitPrice.SalesPrice,
                        PurchasePrice = unitPrice.PurchasePrice,
                        CurrencyCode = currency.Code,
                        SalesVatRate = type == UnitPriceType.Item ? item.SalesVatRate : service.SalesVatRate,
                        PurchaseVatRate = type == UnitPriceType.Item ? item.PurchaseVatRate : service.PurchaseVatRate,
                        ClientCode = client.Code
                    };



        var result = await AsyncExecuter.FirstOrDefaultAsync(query);

        if (result != null)
        {
            var price = isSales ? result.SalesPrice
                                : result.PurchasePrice;

            if (result.IsVatIncluded)
            {
                byte productVatRate = isSales ? result.SalesVatRate : result.PurchaseVatRate;
                price = price / (1 + (decimal)productVatRate / 100);
            }

            if (vatRate.HasValue && vatRate.Value > 0)
                price = price * (1 + ((decimal)vatRate.Value / 100));

            price = Math.Round(price, 5);

            return price;
        }

        return 0;
    }
    
    public async Task<decimal> GetPriceAsync(
        string productCode,
        UnitPriceType type,
        string unitCode,
        DateTime date,
        bool isSales,
        byte? vatRate = default,
        string currencyCode = default,
        string clientCode = default)
    {
        dynamic product = await GetProductAsync(productCode, type);

        UnitPrice result = await GetUnitPriceAsync(
            product,
            type,
            unitCode,
            date,
            currencyCode,
            clientCode);

        if (result != null)
        {
            var price = isSales ? result.SalesPrice
                                : result.PurchasePrice;

            if (result.IsVatIncluded)
            {
                byte productVatRate = isSales
                    ? product.SalesVatRate : product.PurchaseVatRate;
                price = price / (1 + (decimal)productVatRate / 100);
            }

            if (vatRate.HasValue && vatRate.Value > 0)
                price = price * (1 + ((decimal)vatRate.Value / 100));

            price = Math.Round(price, 5);

            return price;
        }

        return 0;
    }

    public async Task<UnitPrice> GetUnitPriceAsync(
        string productCode,
        UnitPriceType type,
        string unitCode,
        DateTime date,
        string currencyCode = default,
        string clientCode = default)
    {
        dynamic product = await GetProductAsync(productCode, type);

        var result = await GetUnitPriceAsync(
            product,
            type,
            unitCode,
            date,
            currencyCode,
            clientCode);

        return result;
    }

    protected async Task<UnitPrice> GetUnitPriceAsync(
        dynamic product,
        UnitPriceType type,
        string unitCode,
        DateTime date,
        string currencyCode = default,
        string clientCode = default)
    {
        Unit unit = await GetUnitAsync(product, unitCode);
        Client client = null; Currency currency = null;

        if (!clientCode.IsNullOrWhiteSpace())
            client = await ClientRepository.GetByCodeAsync(
                clientCode,
                includeDetails: false);

        if (!currencyCode.IsNullOrWhiteSpace())
            currency = await CurrencyRepository.GetByCodeAsync(
                currencyCode,
                includeDetails: false);


        int productId = product.Id;

        var result = (await UnitPriceRepository.GetQueryableAsync())
            .OrderByDescending(x => x.ClientId)
            .FirstOrDefault(unitPrice =>
            unitPrice.Type == type
            && unitPrice.ProductId == productId
            && unitPrice.UnitId == unit.Id
            && date >= unitPrice.BeginDate && date <= unitPrice.EndDate
            && unitPrice.CurrencyId == (currency == null ? null : currency.Id)
            && (client == null
                 ? unitPrice.ClientId == null
                 : (unitPrice.ClientId == null || unitPrice.ClientId == client.Id))
           );

        return result;
    }

    protected async Task<dynamic> GetProductAsync(
        string productCode,
        UnitPriceType type)
    {
        dynamic product = null;

        switch (type)
        {
            case UnitPriceType.Item:
                product = await ItemRepository.GetByCodeAsync(
                    productCode,
                    includeDetails: false);
                break;
            case UnitPriceType.Service:
                product = await ServiceRepository.GetByCodeAsync(
                    productCode,
                    includeDetails: false);
                break;
        }

        return product;
    }

    protected async Task<Unit> GetUnitAsync(
        dynamic product,
        string unitCode)
    {
        int unitGroupId = product.UnitGroupId;

        var unit = await UnitRepository.FirstOrDefaultAsync(
            u => u.Code == unitCode
            && u.UnitGroupId == unitGroupId);

        if (unit == null)
            throw new CodeNotFoundException(typeof(Unit), unitCode);

        return unit;
    }

    #endregion
}
