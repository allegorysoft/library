using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Allegory.Saler.UnitPrices;

public interface IUnitPriceAppService : IApplicationService
{
    Task<PagedResultDto<UnitPriceDto>> ListAsync(FilteredPagedAndSortedResultRequestDto input);
    Task<UnitPriceWithDetailsDto> GetAsync(int id);
    Task<UnitPriceWithDetailsDto> GetByCodeAsync(string code, UnitPriceType type);
    Task<UnitPriceWithDetailsDto> CreateAsync(UnitPriceCreateDto input);
    Task<UnitPriceWithDetailsDto> UpdateAsync(int id, UnitPriceUpdateDto input);
    Task DeleteAsync(int id);
    Task<decimal> GetPriceAsync(
        string productCode,
        UnitPriceType type,
        string unitCode,
        DateTime date,
        bool isSales,
        byte? vatRate = default,
        string currencyCode = default,
        string clientCode = default);
}
