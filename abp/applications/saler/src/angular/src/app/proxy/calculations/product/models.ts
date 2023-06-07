import type { EntityDto } from '@abp/ng.core';

export interface CalculableProductAggregateRootDto {
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
  lines: CalculableProductDto[];
  discounts: DiscountDto[];
}

export interface CalculableProductAggregateRootInputDto {
  currencyCode?: string;
  currencyRate?: number;
  lines: CalculableProductInputDto[];
  discounts: DiscountDto[];
}

export interface CalculableProductDto extends EntityDto<number> {
  quantity: number;
  price: number;
  vatRate: number;
  isVatIncluded: boolean;
  total: number;
  discountTotal: number;
  calculatedTotal: number;
  vatBase: number;
  vatAmount: number;
  discounts: DiscountDto[];
  deductionPart1?: number;
  deductionPart2?: number;
  deductionCode?: string;
  currencyCode?: string;
  currencyRate?: number;
  currencyPrice?: number;
  currencyTotal?: number;
}

export interface CalculableProductInputDto extends EntityDto<number> {
  quantity: number;
  price: number;
  vatRate: number;
  isVatIncluded: boolean;
  total: number;
  discounts: DiscountDto[];
  deductionPart1?: number;
  deductionPart2?: number;
  deductionCode?: string;
  currencyCode?: string;
  currencyRate?: number;
  currencyPrice?: number;
  currencyTotal?: number;
}

export interface DeductionDto extends EntityDto<number> {
  deductionCode?: string;
  deductionName?: string;
  deductionPart1?: number;
  deductionPart2?: number;
}

export interface DiscountDto extends EntityDto<number> {
  id?: number;
  rate: number;
  total: number;
}
