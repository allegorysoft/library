using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Allegory.Saler.Orders;

public interface IOrderAppService : IApplicationService
{
    Task<PagedResultDto<OrderDto>> ListAsync(FilteredPagedAndSortedResultRequestDto input);
    Task<OrderWithDetailsDto> GetAsync(int id);
    Task<OrderWithDetailsDto> GetByNumberAsync(string number, OrderType type);
    Task<OrderWithDetailsDto> CreateAsync(OrderCreateDto input);
    Task<OrderWithDetailsDto> UpdateAsync(int id, OrderUpdateDto input);
    Task DeleteAsync(int id);
}
