import { mapEnumToOptions } from '@abp/ng.core';

export enum UnitPriceType {
  Item = 0,
  Service = 1,
}

export const unitPriceTypeOptions = mapEnumToOptions(UnitPriceType);
