using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Allegory.Saler.Clients;

public interface IClientAppService : IApplicationService
{
    Task<PagedResultDto<ClientDto>> ListAsync(FilteredPagedAndSortedResultRequestDto input);
    Task<ClientWithDetailsDto> GetAsync(int id);
    Task<ClientWithDetailsDto> GetByCodeAsync(string code);
    Task<ClientWithDetailsDto> CreateAsync(ClientCreateDto input);
    Task<ClientWithDetailsDto> UpdateAsync(int id, ClientUpdateDto input);
    Task DeleteAsync(int id);
}
