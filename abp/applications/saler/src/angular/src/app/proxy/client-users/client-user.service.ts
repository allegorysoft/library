import { RestService } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ClientUserService {
  apiName = 'Default';

  addUserByClientIdAndUserId = (clientId: number, userId: string) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/client-user/user',
      params: { clientId, userId },
    },
    { apiName: this.apiName });

  removeUserByUserId = (userId: string) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/client-user/user/${userId}`,
    },
    { apiName: this.apiName });

  constructor(private restService: RestService) {}
}
