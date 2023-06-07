import { mapEnumToOptions } from '@abp/ng.core';

export enum OrderStatu {
  Offer = 0,
  Approved = 1,
  Closed = 2,
  Cancelled = 3,
}

export const orderStatuOptions = mapEnumToOptions(OrderStatu);
