import type { GetServiceLookupListDto, ServiceCreateDto, ServiceDto, ServiceLookupDto, ServiceUpdateDto, ServiceWithDetailsDto } from './models';
import { RestService } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import { FilteredPagedAndSortedResultRequestDto } from '@proxy';

@Injectable({
  providedIn: 'root',
})
export class ServiceService {
  apiName = 'Default';

  create = (input: ServiceCreateDto) =>
    this.restService.request<any, ServiceWithDetailsDto>({
      method: 'POST',
      url: '/api/app/service',
      body: input,
    },
      { apiName: this.apiName });

  delete = (id: number) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/service/${id}`,
    },
      { apiName: this.apiName });

  get = (id: number) =>
    this.restService.request<any, ServiceWithDetailsDto>({
      method: 'GET',
      url: `/api/app/service/${id}`,
    },
      { apiName: this.apiName });

  getByCode = (code: string) =>
    this.restService.request<any, ServiceWithDetailsDto>({
      method: 'GET',
      url: '/api/app/service/by-code',
      params: { code },
    },
      { apiName: this.apiName });

  list = (input: FilteredPagedAndSortedResultRequestDto) =>
    this.restService.request<any, PagedResultDto<ServiceDto>>({
      method: 'POST',
      url: '/api/app/service/list',
      body: input,
    },
      { apiName: this.apiName });

  listServiceLookup = (input: GetServiceLookupListDto) =>
    this.restService.request<any, PagedResultDto<ServiceLookupDto>>({
      method: 'POST',
      url: '/api/app/service/list-service-lookup',
      body: input,
    },
      { apiName: this.apiName });

  update = (id: number, input: ServiceUpdateDto) =>
    this.restService.request<any, ServiceWithDetailsDto>({
      method: 'PUT',
      url: `/api/app/service/${id}`,
      body: input,
    },
      { apiName: this.apiName });

  constructor(private restService: RestService) { }
}
