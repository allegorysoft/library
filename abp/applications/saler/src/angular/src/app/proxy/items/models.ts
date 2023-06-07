import type { ExtensibleAuditedEntityDto, ExtensibleEntityDto } from '@abp/ng.core';
import type { ItemType } from './item-type.enum';
import { FilteredPagedAndSortedResultRequestDto } from '@proxy';

export interface GetItemLookupListDto extends FilteredPagedAndSortedResultRequestDto {
  date: string;
  isSales: boolean;
  clientCode?: string;
}

export interface ItemCreateDto extends ItemCreateOrUpdateDtoBase {
  type: ItemType;
}

export interface ItemCreateOrUpdateDtoBase extends ExtensibleEntityDto<number> {
  code: string;
  name?: string;
  unitGroupCode: string;
  deductionCode?: string;
  salesDeductionPart1?: number;
  salesDeductionPart2?: number;
  purchaseDeductionPart1?: number;
  purchaseDeductionPart2?: number;
  salesVatRate: number;
  purchaseVatRate: number;
}

export interface ItemDto extends ExtensibleAuditedEntityDto<number> {
  mainUnitCode?: string;
  code?: string;
  name?: string;
  type: ItemType;
  stock?: number;
  reservedStock?: number;
}

export interface ItemLookupDto extends ItemDto {
  price?: number;
  vatIncludedPrice?: number;
}

export interface ItemUpdateDto extends ItemCreateOrUpdateDtoBase {
}

export interface ItemWithDetailsDto extends ExtensibleAuditedEntityDto<number> {
  unitGroupCode?: string;
  code?: string;
  name?: string;
  type: ItemType;
  deductionCode?: string;
  salesDeductionPart1?: number;
  salesDeductionPart2?: number;
  purchaseDeductionPart1?: number;
  purchaseDeductionPart2?: number;
  salesVatRate: number;
  purchaseVatRate: number;
}
