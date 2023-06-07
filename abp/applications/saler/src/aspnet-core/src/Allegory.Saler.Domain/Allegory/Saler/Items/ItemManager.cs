using Allegory.Saler.Calculations.Product;
using Allegory.Saler.Orders;
using Allegory.Saler.UnitPrices;
using Allegory.Saler.Units;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Allegory.Saler.Items;

public class ItemManager : SalerDomainService
{
    protected IItemRepository ItemRepository { get; }
    protected IUnitGroupRepository UnitGroupRepository { get; }
    protected IReadOnlyRepository<OrderLine, int> OrderLineRepository => LazyServiceProvider.LazyGetRequiredService<IReadOnlyRepository<OrderLine, int>>();
    protected IUnitPriceRepository UnitPriceRepository => LazyServiceProvider.LazyGetRequiredService<IUnitPriceRepository>();
    protected DeductionManager DeductionManager =>
    LazyServiceProvider.LazyGetRequiredService<DeductionManager>();

    public ItemManager(
        IItemRepository itemRepository,
        IUnitGroupRepository unitGroupRepository)
    {
        ItemRepository = itemRepository;
        UnitGroupRepository = unitGroupRepository;
    }

    public async Task<Item> CreateAsync(
        string code,
        ItemType type,
        string unitGroupCode,
        string name = default)
    {
        await CheckItemExistsAsync(code);

        var unitGroup = await UnitGroupRepository.GetByCodeAsync(unitGroupCode, includeDetails: false);

        var item = new Item(
            code,
            type,
            unitGroup.Id,
            name: name);

        return item;
    }

    public async Task CheckItemExistsAsync(
        string code,
        int? itemId = default)
    {
        Expression<Func<Item, bool>> expression = item => item.Code == code;

        if (itemId != default)
            expression = expression.And(item => item.Id != itemId);

        var itemExists = await ItemRepository.AnyAsync(expression);

        if (itemExists)
            throw new CodeAlreadyExistsException(typeof(Item), code);
    }

    public async Task ChangeCodeAsync(
        Item item,
        string newCode)
    {
        await CheckItemExistsAsync(newCode, item.Id);
        item.ChangeCode(newCode);
    }

    public async Task ChangeUnitGroupAsync(
        Item item,
        string unitGroupCode)
    {
        var unitGroup = await UnitGroupRepository.GetByCodeAsync(unitGroupCode, includeDetails: false);
        if (unitGroup.Id == item.UnitGroupId)
            return;

        //TODO Check InvoiceLine, DispatchLine too 
        if (await OrderLineRepository.AnyAsync(orderLine => orderLine.Type == OrderLineType.Item && orderLine.ProductId == item.Id))
            throw new ThereIsTransactionRecordException(typeof(UnitGroup), typeof(Order));

        if (await UnitPriceRepository.AnyAsync(unitPrice => unitPrice.Type == UnitPriceType.Item && unitPrice.ProductId == item.Id))
            throw new ThereIsTransactionRecordException(typeof(UnitGroup), typeof(UnitPrice));

        item.UnitGroupId = unitGroup.Id;
    }

    public void SetDeduction(
        Item item,
        string deductionCode,
        short? salesDeductionPart1,
        short? salesDeductionPart2,
        short? purchaseDeductionPart1,
        short? purchaseDeductionPart2)
    {
        if (!string.IsNullOrEmpty(deductionCode)
            && (!(salesDeductionPart1.HasValue && salesDeductionPart2.HasValue)
                && !(purchaseDeductionPart1.HasValue && purchaseDeductionPart1.HasValue)))
            throw new BusinessException(SalerDomainErrorCodes.DeductionWrong);

        if (salesDeductionPart1.HasValue || salesDeductionPart2.HasValue)
            DeductionManager.CheckDeduction(
                deductionCode,
                salesDeductionPart1,
                salesDeductionPart2);

        if (purchaseDeductionPart1.HasValue || purchaseDeductionPart1.HasValue)
            DeductionManager.CheckDeduction(
                deductionCode,
                purchaseDeductionPart1,
                purchaseDeductionPart2);

        item.SetDeduction(
            deductionCode,
            salesDeductionPart1,
            salesDeductionPart2,
            purchaseDeductionPart1,
            purchaseDeductionPart2);
    }
}
