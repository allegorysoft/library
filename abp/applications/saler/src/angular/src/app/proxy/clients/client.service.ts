import type { ClientCreateDto, ClientDto, ClientUpdateDto } from './models';
import { RestService } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import { FilteredPagedAndSortedResultRequestDto } from '@proxy';

@Injectable({
  providedIn: 'root',
})
export class ClientService {
  apiName = 'Default';

  create = (input: ClientCreateDto) =>
    this.restService.request<any, ClientDto>({
      method: 'POST',
      url: '/api/app/client',
      body: input,
    },
      { apiName: this.apiName });

  delete = (id: number) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/client/${id}`,
    },
      { apiName: this.apiName });

  get = (id: number) =>
    this.restService.request<any, ClientDto>({
      method: 'GET',
      url: `/api/app/client/${id}`,
    },
      { apiName: this.apiName });

  getByCode = (code: string) =>
    this.restService.request<any, ClientDto>({
      method: 'GET',
      url: '/api/app/client/by-code',
      params: { code },
    },
      { apiName: this.apiName });

  list = (input: FilteredPagedAndSortedResultRequestDto) =>
    this.restService.request<any, PagedResultDto<ClientDto>>({
      method: 'POST',
      url: '/api/app/client/list',
      body: input,
    },
      { apiName: this.apiName });

  update = (id: number, input: ClientUpdateDto) =>
    this.restService.request<any, ClientDto>({
      method: 'PUT',
      url: `/api/app/client/${id}`,
      body: input,
    },
      { apiName: this.apiName });

  constructor(private restService: RestService) { }
}
