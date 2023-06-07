using Allegory.Standart.Filter.Concrete;

namespace Volo.Abp.Application.Dtos;

public interface IFilteredPagedAndSortedResultRequest : IPagedAndSortedResultRequest
{
    Condition Conditions { get; set; }
}
