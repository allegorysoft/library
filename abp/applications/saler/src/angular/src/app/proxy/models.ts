import type { PagedAndSortedResultRequestDto } from '@abp/ng.core';
import { Condition } from '@allegorysoft/filter';

export interface FilteredPagedAndSortedResultRequestDto extends PagedAndSortedResultRequestDto {
  conditions: Condition;
}
