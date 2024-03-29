import { Injectable } from '@angular/core';
import { RestService } from '@abp/ng.core';

@Injectable({
  providedIn: 'root',
})
export class MyProjectNameAdminService {
  apiName = 'MyProjectNameAdmin';

  constructor(private restService: RestService) { }

  sample() {
    return this.restService.request<void, any>(
      { method: 'GET', url: '/api/MyProjectName/admin/sample' },
      { apiName: this.apiName }
    );
  }
}
