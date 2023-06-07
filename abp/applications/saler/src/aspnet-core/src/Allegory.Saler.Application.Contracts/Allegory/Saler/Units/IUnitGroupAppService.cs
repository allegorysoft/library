using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Allegory.Saler.Units;

public interface IUnitGroupAppService : IApplicationService
{
    Task<PagedResultDto<UnitGroupDto>> ListAsync(FilteredPagedAndSortedResultRequestDto input);
    Task<UnitGroupWithDetailsDto> GetAsync(int id);
    Task<UnitGroupWithDetailsDto> GetByCodeAsync(string code);
    Task<UnitGroupWithDetailsDto> CreateAsync(UnitGroupCreateDto input);
    Task<UnitGroupWithDetailsDto> UpdateAsync(int id, UnitGroupUpdateDto input);
    Task DeleteAsync(int id);
    IList<GlobalUnitDto> GetGlobalUnits();
}
