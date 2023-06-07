import type { EntityDto } from '@abp/ng.core';

export interface CurrencyCreateUpdateDto extends EntityDto<number> {
  code: string;
  name?: string;
  symbol?: string;
}

export interface CurrencyDailyExchangeCreateUpdateDto extends EntityDto<number> {
  currencyCode: string;
  date: string;
  rate1: number;
  rate2: number;
  rate3: number;
  rate4: number;
}

export interface CurrencyDailyExchangeDto extends EntityDto<number> {
  currencyCode?: string;
  rate1: number;
  rate2: number;
  rate3: number;
  rate4: number;
}

export interface CurrencyDto extends EntityDto<number> {
  code?: string;
  name?: string;
  symbol?: string;
}
