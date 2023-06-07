using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Allegory.Saler.Services;

public interface IServiceAppService : IApplicationService
{
    Task<PagedResultDto<ServiceDto>> ListAsync(FilteredPagedAndSortedResultRequestDto input);
    Task<PagedResultDto<ServiceLookupDto>> ListServiceLookupAsync(GetServiceLookupListDto input);
    Task<ServiceWithDetailsDto> GetAsync(int id);
    Task<ServiceWithDetailsDto> GetByCodeAsync(string code);
    Task<ServiceWithDetailsDto> CreateAsync(ServiceCreateDto input);
    Task<ServiceWithDetailsDto> UpdateAsync(int id, ServiceUpdateDto input);
    Task DeleteAsync(int id);
}
