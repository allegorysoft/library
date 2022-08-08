import { Injectable } from '@angular/core';
import { RestService } from '@abp/ng.core';

@Injectable({
  providedIn: 'root',
})
export class MyProjectNameCommonService {
  apiName = 'MyProjectNameCommon';

  constructor(private restService: RestService) { }

  sample() {
    return this.restService.request<void, any>(
      { method: 'GET', url: '/api/MyProjectName/common/sample' },
      { apiName: this.apiName }
    );
  }
}
