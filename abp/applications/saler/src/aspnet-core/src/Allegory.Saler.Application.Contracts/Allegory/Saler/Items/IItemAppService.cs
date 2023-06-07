using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Allegory.Saler.Items;

public interface IItemAppService : IApplicationService
{
    Task<PagedResultDto<ItemDto>> ListAsync(FilteredPagedAndSortedResultRequestDto input);
    Task<PagedResultDto<ItemLookupDto>> ListItemLookupAsync(GetItemLookupListDto input);
    Task<ItemWithDetailsDto> GetAsync(int id);
    Task<ItemWithDetailsDto> GetByCodeAsync(string code);
    Task<ItemWithDetailsDto> CreateAsync(ItemCreateDto input);
    Task<ItemWithDetailsDto> UpdateAsync(int id, ItemUpdateDto input);
    Task DeleteAsync(int id);
}
