import { mapEnumToOptions } from '@abp/ng.core';

export enum OrderType {
  Purchase = 0,
  Sales = 1,
}

export const orderTypeOptions = mapEnumToOptions(OrderType);
