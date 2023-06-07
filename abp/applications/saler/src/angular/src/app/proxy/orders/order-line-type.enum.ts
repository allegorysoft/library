import { mapEnumToOptions } from '@abp/ng.core';

export enum OrderLineType {
  Item = 0,
  Service = 1,
}

export const orderLineTypeOptions = mapEnumToOptions(OrderLineType);
