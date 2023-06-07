import type { VersionInfo } from './models';
import { RestService } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class HomeService {
  apiName = 'Default';

  version = () =>
    this.restService.request<any, VersionInfo>({
      method: 'GET',
      url: '/version',
    },
    { apiName: this.apiName });

  constructor(private restService: RestService) {}
}
