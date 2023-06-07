import type { ExtensibleAuditedEntityDto, ExtensibleEntityDto } from '@abp/ng.core';
import { FilteredPagedAndSortedResultRequestDto } from '@proxy';

export interface GetServiceLookupListDto extends FilteredPagedAndSortedResultRequestDto {
  date: string;
  isSales: boolean;
  clientCode?: string;
}

export interface ServiceCreateDto extends ServiceCreateOrUpdateDtoBase {
}

export interface ServiceCreateOrUpdateDtoBase extends ExtensibleEntityDto<number> {
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

export interface ServiceDto extends ExtensibleAuditedEntityDto<number> {
  mainUnitCode?: string;
  code?: string;
  name?: string;
}

export interface ServiceLookupDto extends ServiceDto {
  price?: number;
  vatIncludedPrice?: number;
}

export interface ServiceUpdateDto extends ServiceCreateOrUpdateDtoBase {
}

export interface ServiceWithDetailsDto extends ExtensibleAuditedEntityDto<number> {
  unitGroupCode?: string;
  code?: string;
  name?: string;
  deductionCode?: string;
  salesDeductionPart1?: number;
  salesDeductionPart2?: number;
  purchaseDeductionPart1?: number;
  purchaseDeductionPart2?: number;
  salesVatRate: number;
  purchaseVatRate: number;
}
