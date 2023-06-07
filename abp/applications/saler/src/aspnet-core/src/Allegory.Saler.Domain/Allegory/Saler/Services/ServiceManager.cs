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

namespace Allegory.Saler.Services;

public class ServiceManager : SalerDomainService
{
    protected IServiceRepository ServiceRepository { get; }
    protected IUnitGroupRepository UnitGroupRepository { get; }
    protected IUnitPriceRepository UnitPriceRepository => LazyServiceProvider.LazyGetRequiredService<IUnitPriceRepository>();
    protected IReadOnlyRepository<OrderLine, int> OrderLineRepository => LazyServiceProvider.LazyGetRequiredService<IReadOnlyRepository<OrderLine, int>>();
    protected DeductionManager DeductionManager =>
  LazyServiceProvider.LazyGetRequiredService<DeductionManager>();

    public ServiceManager(
        IServiceRepository serviceRepository,
        IUnitGroupRepository unitGroupRepository)
    {
        ServiceRepository = serviceRepository;
        UnitGroupRepository = unitGroupRepository;
    }

    public async Task<Service> CreateAsync(
        string code,
        string unitGroupCode,
        string name = default)
    {
        await CheckServiceExistsAsync(code);

        var unitGroup = await UnitGroupRepository.GetByCodeAsync(unitGroupCode, includeDetails: false);

        var service = new Service(
            code,
            unitGroup.Id,
            name: name);

        return service;
    }

    public async Task CheckServiceExistsAsync(
        string code,
        int? serviceId = default)
    {
        Expression<Func<Service, bool>> expression = service => service.Code == code;

        if (serviceId != default)
            expression = expression.And(service => service.Id != serviceId);

        var serviceExists = await ServiceRepository.AnyAsync(expression);

        if (serviceExists)
            throw new CodeAlreadyExistsException(typeof(Service), code);
    }

    public async Task ChangeCodeAsync(
        Service service,
        string newCode)
    {
        await CheckServiceExistsAsync(newCode, service.Id);
        service.ChangeCode(newCode);
    }

    public async Task ChangeUnitGroupAsync(
        Service service,
        string unitGroupCode)
    {
        var unitGroup = await UnitGroupRepository.GetByCodeAsync(unitGroupCode, includeDetails: false);
        if (unitGroup.Id == service.UnitGroupId)
            return;

        //TODO Check InvoiceLine, DispatchLine too 
        if (await OrderLineRepository.AnyAsync(orderLine => orderLine.Type == OrderLineType.Service && orderLine.ProductId == service.Id))
            throw new ThereIsTransactionRecordException(typeof(UnitGroup), typeof(Order));

        if (await UnitPriceRepository.AnyAsync(unitPrice => unitPrice.Type == UnitPriceType.Service && unitPrice.ProductId == service.Id))
            throw new ThereIsTransactionRecordException(typeof(UnitGroup), typeof(UnitPrice));

        service.UnitGroupId = unitGroup.Id;
    }

    public void SetDeduction(
        Service service,
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

        service.SetDeduction(
            deductionCode,
            salesDeductionPart1,
            salesDeductionPart2,
            purchaseDeductionPart1,
            purchaseDeductionPart2);
    }
}
