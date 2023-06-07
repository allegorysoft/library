using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Allegory.Saler.Calculations.Product;

public interface IProductCalculatorAppService : IApplicationService
{
    Task<CalculableProductAggregateRootDto> CalculateAggregateRootAsync(CalculableProductAggregateRootInputDto input);
    Task<CalculableProductDto> CalculateAsync(CalculableProductInputDto input);
    IList<DeductionDto> GetDeductions();
}
