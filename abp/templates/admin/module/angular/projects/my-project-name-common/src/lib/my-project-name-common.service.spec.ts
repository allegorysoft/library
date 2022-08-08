import { TestBed } from '@angular/core/testing';

import { MyProjectNameCommonService } from './services/my-project-name-common.service';

describe('MyProjectNameCommonService', () => {
  let service: MyProjectNameCommonService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MyProjectNameCommonService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
