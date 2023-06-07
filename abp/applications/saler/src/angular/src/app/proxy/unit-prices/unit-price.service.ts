import type { UnitPriceCreateDto, UnitPriceDto, UnitPriceUpdateDto, UnitPriceWithDetailsDto } from './models';
import type { UnitPriceType } from './unit-price-type.enum';
import { RestService } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import { FilteredPagedAndSortedResultRequestDto } from '@proxy';

@Injectable({
  providedIn: 'root',
})
export class UnitPriceService {
  apiName = 'Default';

  create = (input: UnitPriceCreateDto) =>
    this.restService.request<any, UnitPriceWithDetailsDto>({
      method: 'POST',
      url: '/api/app/unit-price',
      body: input,
    },
      { apiName: this.apiName });

  delete = (id: number) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/unit-price/${id}`,
    },
      { apiName: this.apiName });

  get = (id: number) =>
    this.restService.request<any, UnitPriceWithDetailsDto>({
      method: 'GET',
      url: `/api/app/unit-price/${id}`,
    },
      { apiName: this.apiName });

  getByCode = (code: string, type: UnitPriceType) =>
    this.restService.request<any, UnitPriceWithDetailsDto>({
      method: 'GET',
      url: '/api/app/unit-price/by-code',
      params: { code, type },
    },
      { apiName: this.apiName });

  getPrice = (productCode: string, type: UnitPriceType, unitCode: string, date: string, isSales: boolean, vatRate?: number, currencyCode?: string, clientCode?: string) =>
    this.restService.request<any, number>({
      method: 'GET',
      url: '/api/app/unit-price/price',
      params: { productCode, type, unitCode, date, isSales, vatRate, currencyCode, clientCode },
    },
      { apiName: this.apiName });

  list = (input: FilteredPagedAndSortedResultRequestDto) =>
    this.restService.request<any, PagedResultDto<UnitPriceDto>>({
      method: 'POST',
      url: '/api/app/unit-price/list',
      body: input,
    },
      { apiName: this.apiName });

  update = (id: number, input: UnitPriceUpdateDto) =>
    this.restService.request<any, UnitPriceWithDetailsDto>({
      method: 'PUT',
      url: `/api/app/unit-price/${id}`,
      body: input,
    },
      { apiName: this.apiName });

  constructor(private restService: RestService) { }
}
