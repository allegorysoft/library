import type { OrderCreateDto, OrderDto, OrderUpdateDto, OrderWithDetailsDto } from './models';
import type { OrderType } from './order-type.enum';
import { RestService } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import { FilteredPagedAndSortedResultRequestDto } from '@proxy';

@Injectable({
  providedIn: 'root',
})
export class OrderService {
  apiName = 'Default';

  create = (input: OrderCreateDto) =>
    this.restService.request<any, OrderWithDetailsDto>({
      method: 'POST',
      url: '/api/app/order',
      body: input,
    },
      { apiName: this.apiName });

  delete = (id: number) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/order/${id}`,
    },
      { apiName: this.apiName });

  get = (id: number) =>
    this.restService.request<any, OrderWithDetailsDto>({
      method: 'GET',
      url: `/api/app/order/${id}`,
    },
      { apiName: this.apiName });

  getByNumber = (number: string, type: OrderType) =>
    this.restService.request<any, OrderWithDetailsDto>({
      method: 'GET',
      url: '/api/app/order/by-number',
      params: { number, type },
    },
      { apiName: this.apiName });

  list = (input: FilteredPagedAndSortedResultRequestDto) =>
    this.restService.request<any, PagedResultDto<OrderDto>>({
      method: 'POST',
      url: '/api/app/order/list',
      body: input,
    },
      { apiName: this.apiName });

  update = (id: number, input: OrderUpdateDto) =>
    this.restService.request<any, OrderWithDetailsDto>({
      method: 'PUT',
      url: `/api/app/order/${id}`,
      body: input,
    },
      { apiName: this.apiName });

  constructor(private restService: RestService) { }
}
