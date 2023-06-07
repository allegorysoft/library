using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Allegory.Saler.Items;

public class GetItemLookupListDto : FilteredPagedAndSortedResultRequestDto
{
    [Required]
    public DateTime Date { get; set; }

    public bool IsSales { get; set; }
    public string ClientCode { get; set; }
}
