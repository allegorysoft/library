import type { OrderType } from './order-type.enum';
import type { CalculableProductDto, CalculableProductInputDto, DiscountDto } from '../calculations/product/models';
import type { ExtensibleAuditedEntityDto, ExtensibleEntityDto } from '@abp/ng.core';
import type { OrderStatu } from './order-statu.enum';
import type { OrderLineType } from './order-line-type.enum';

export interface OrderCreateDto extends OrderCreateOrUpdateDtoBase {
  type: OrderType;
  lines: OrderLineCreateDto[];
  discounts: DiscountDto[];
  currencyCode?: string;
  currencyRate?: number;
}

export interface OrderCreateOrUpdateDtoBase extends ExtensibleEntityDto<number> {
  number: string;
  statu: OrderStatu;
  clientCode?: string;
  date: string;
}

export interface OrderDto extends ExtensibleAuditedEntityDto<number> {
  number?: string;
  type: OrderType;
  statu: OrderStatu;
  clientCode?: string;
  date?: string;
  totalGross: number;
  currencyTotalGross?: number;
  currencySymbol?: string;
}

export interface OrderLineCreateDto extends OrderLineCreateOrUpdateDtoBase {
}

export interface OrderLineCreateOrUpdateDtoBase extends CalculableProductInputDto {
  type: OrderLineType;
  productCode: string;
  unitCode: string;
  reserveDate?: string;
  reserveQuantity?: number;
}

export interface OrderLineDto extends CalculableProductDto {
  id: number;
  type: OrderLineType;
  productCode?: string;
  productName?: string;
  unitGroupCode?: string;
  unitCode?: string;
  reserveDate?: string;
  reserveQuantity?: number;
}

export interface OrderLineUpdateDto extends OrderLineCreateOrUpdateDtoBase {
  id?: number;
}

export interface OrderUpdateDto extends OrderCreateOrUpdateDtoBase {
  lines: OrderLineUpdateDto[];
  discounts: DiscountDto[];
  currencyCode?: string;
  currencyRate?: number;
}

export interface OrderWithDetailsDto extends ExtensibleAuditedEntityDto<number> {
  number?: string;
  type: OrderType;
  date?: string;
  statu: OrderStatu;
  clientCode?: string;
  totalDiscount: number;
  totalVatBase: number;
  totalVatAmount: number;
  totalGross: number;
  currencyCode?: string;
  currencyRate?: number;
  currencyTotalDiscount?: number;
  currencyTotalVatBase?: number;
  currencyTotalVatAmount?: number;
  currencyTotalGross?: number;
  lines: OrderLineDto[];
  discounts: DiscountDto[];
}
