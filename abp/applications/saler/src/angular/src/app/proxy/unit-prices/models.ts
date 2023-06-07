import type { UnitPriceType } from './unit-price-type.enum';
import type { ExtensibleAuditedEntityDto, ExtensibleEntityDto } from '@abp/ng.core';

export interface UnitPriceCreateDto extends UnitPriceCreateOrUpdateDtoBase {
  type: UnitPriceType;
}

export interface UnitPriceCreateOrUpdateDtoBase extends ExtensibleEntityDto<number> {
  code: string;
  productCode: string;
  unitCode: string;
  currencyCode?: string;
  salesPrice: number;
  purchasePrice: number;
  beginDate: string;
  endDate: string;
  isVatIncluded: boolean;
  clientCode?: string;
}

export interface UnitPriceDto extends ExtensibleAuditedEntityDto<number> {
  code?: string;
  type: UnitPriceType;
  productCode?: string;
  productName?: string;
  unitCode?: string;
  purchasePrice: number;
  salesPrice: number;
  currencyCode?: string;
  clientCode?: string;
  isVatIncluded: boolean;
}

export interface UnitPriceUpdateDto extends UnitPriceCreateOrUpdateDtoBase {
}

export interface UnitPriceWithDetailsDto extends ExtensibleAuditedEntityDto<number> {
  code?: string;
  type: UnitPriceType;
  productCode?: string;
  productName?: string;
  unitCode?: string;
  purchasePrice: number;
  salesPrice: number;
  currencyCode?: string;
  clientCode?: string;
  isVatIncluded: boolean;
  beginDate?: string;
  endDate?: string;
}
