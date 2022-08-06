import { Injectable } from '@angular/core';
import { RestService } from '@abp/ng.core';

@Injectable({
  providedIn: 'root',
})
export class MyProjectNamePublicService {
  apiName = 'MyProjectNamePublic';

  constructor(private restService: RestService) { }

  sample() {
    return this.restService.request<void, any>(
      { method: 'GET', url: '/api/MyProjectName/public/sample' },
      { apiName: this.apiName }
    );
  }
}
