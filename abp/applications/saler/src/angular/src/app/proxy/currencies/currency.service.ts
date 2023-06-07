import type { CurrencyCreateUpdateDto, CurrencyDailyExchangeCreateUpdateDto, CurrencyDailyExchangeDto, CurrencyDto } from './models';
import { RestService } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import { FilteredPagedAndSortedResultRequestDto } from '@proxy';


@Injectable({
  providedIn: 'root',
})
export class CurrencyService {
  apiName = 'Default';

  create = (input: CurrencyCreateUpdateDto) =>
    this.restService.request<any, CurrencyDto>({
      method: 'POST',
      url: '/api/app/currency',
      body: input,
    },
      { apiName: this.apiName });

  delete = (id: number) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/currency/${id}`,
    },
      { apiName: this.apiName });

  editCurrencyDailyExchange = (input: CurrencyDailyExchangeCreateUpdateDto) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/currency/edit-currency-daily-exchange',
      body: input,
    },
      { apiName: this.apiName });

  get = (id: number) =>
    this.restService.request<any, CurrencyDto>({
      method: 'GET',
      url: `/api/app/currency/${id}`,
    },
      { apiName: this.apiName });

  getByCode = (code: string) =>
    this.restService.request<any, CurrencyDto>({
      method: 'GET',
      url: '/api/app/currency/by-code',
      params: { code },
    },
      { apiName: this.apiName });

  getCurrencyDailyExchange = (currencyCode: string, date: string) =>
    this.restService.request<any, CurrencyDailyExchangeDto>({
      method: 'GET',
      url: '/api/app/currency/currency-daily-exchange',
      params: { currencyCode, date },
    },
      { apiName: this.apiName });

  getCurrencyDailyExchangeList = (date: string) =>
    this.restService.request<any, CurrencyDailyExchangeDto[]>({
      method: 'GET',
      url: '/api/app/currency/currency-daily-exchange-list',
      params: { date },
    },
      { apiName: this.apiName });

  list = (input: FilteredPagedAndSortedResultRequestDto) =>
    this.restService.request<any, PagedResultDto<CurrencyDto>>({
      method: 'POST',
      url: '/api/app/currency/list',
      body: input,
    },
      { apiName: this.apiName });

  refreshDailyExchanges = () =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/currency/refresh-daily-exchanges',
    },
      { apiName: this.apiName });

  update = (id: number, input: CurrencyCreateUpdateDto) =>
    this.restService.request<any, CurrencyDto>({
      method: 'PUT',
      url: `/api/app/currency/${id}`,
      body: input,
    },
      { apiName: this.apiName });

  constructor(private restService: RestService) { }
}
