using Allegory.Standart.Filter.Concrete;

namespace Volo.Abp.Application.Dtos;

public class FilteredPagedAndSortedResultRequestDto : PagedAndSortedResultRequestDto, IFilteredPagedAndSortedResultRequest
{
    public Condition Conditions { get; set; }
}
