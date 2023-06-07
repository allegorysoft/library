import { mapEnumToOptions } from '@abp/ng.core';

export enum ItemType {
  Item = 0,
  RawMaterial = 1,
  SemiProduct = 2,
  EndProduct = 3,
}

export const itemTypeOptions = mapEnumToOptions(ItemType);
